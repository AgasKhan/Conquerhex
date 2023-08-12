using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransparentMaterial : MonoBehaviour
{
    [SerializeField]
    protected Renderer originalSprite;

    [SerializeField]
    protected bool isTransparent = true;

    [SerializeField]
    protected Material transparentMaterial;

    [SerializeField]
    Texture mainTexture;

    [SerializeField]
    bool defaultRight = true;

    [SerializeField]
    Color damaged1 = new Color() { r = 1, b = 0, g = 1, a = 1 };

    [SerializeField]
    Color damaged2 = new Color() { r = 1, b = 0.92f, g = 0.016f, a = 1 };

    [SerializeField]
    Color detected = new Color() { r = 1, b = 0.5f, g = 0.5f, a = 1 };

    public ComplexColor colorSetter = new ComplexColor();

    [SerializeField]
    bool proyection=false;

    [SerializeField, Range(0f, 2f)]
    float _shakeIntensity;

    [SerializeField, Range(0f, 2f)]
    float _shakeDuration;

    [SerializeField]
    GameObject onDeathParticlePrefab;

    Vector2Int indexParticle;

    ParticleSystem onDeathParticle;

    Vector3 _initialPosition;

    TimedCompleteAction shakeManager;

    TimedCompleteAction timDamaged = null;

    TimedLerp<Color> timDetected = null;

    TransparentMaterial[] proyections = new TransparentMaterial[6];

    protected virtual void Awake()
    {
        originalSprite.material = transparentMaterial;

        originalSprite.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);

        _initialPosition = transform.localPosition;

        if (mainTexture!=null)
            originalSprite.material.SetTexture("_MainTex", mainTexture);

        if (!(originalSprite is SpriteRenderer))
            return;

        SpriteRenderer originalSpriteRenderer = (SpriteRenderer)originalSprite;

        colorSetter.setter += ColorSetter_setter;

        colorSetter.Add(originalSpriteRenderer.color);

        if (transform.parent == null || proyection)
            return;

        for(int i = 0; i < proyections.Length; i++)
        {
            proyections[i] = CloneAndSuscribe();
        }

        LoadSystem.AddPostLoadCorutine(
        () =>
        {
            GetComponentInParent<Hexagone>(true)?.SetProyections(transform, proyections, true);
            
            for (int i = 0; i < proyections.Length; i++)
            {
                proyections[i].transform.localScale = transform.parent.localScale;
            }
        });

        if (!transform.parent.TryGetComponent(out Entity entity))
        {
            return;
        }

        entity.rend = this;

        entity.onTakeDamage += ShakeSprite;

        entity.onDetected += Entity_onDetected;

        shakeManager = (TimedCompleteAction)TimersManager.Create(_shakeDuration, Shake, EndShake).Stop();

        timDetected = TimersManager.Create(detected, Color.white, 0.1f, Color.Lerp, ChangeColor);

        timDamaged = TimersManager.Create(0.33f, () => ColorBlink(((int)(timDamaged.Percentage() * 10)) % 2 == 0), ColorBlinkEnd);

        if (entity is DynamicEntity)
        {
            var entityDyn = ((DynamicEntity)entity);

            entityDyn.move.onMove += Move_onMove;
        }

        if (onDeathParticlePrefab != null)
        {
            indexParticle = PoolManager.SrchInCategory("Particles", onDeathParticlePrefab.name);
            entity.health.death += Health_death;
        }

        LoadSystem.AddPostLoadCorutine(
        () => {
            for (int i = 0; i < entity.carlitos.Length; i++)
            {
                proyections[i].transform.SetParent(entity.carlitos[i].transform);
                proyections[i].transform.localPosition = Vector3.zero;
            }
        });

    }

    public virtual TransparentMaterial CloneAndSuscribe()
    {
        TransparentMaterial transparentMaterial = Instantiate(this);

        transparentMaterial.proyection = true;

        ((SpriteRenderer)originalSprite).RegisterSpriteChangeCallback(transparentMaterial.UpdateSprite);

        shakeManager?.AddToUpdate(transparentMaterial.Shake).AddToEnd(transparentMaterial.EndShake);

        timDetected?.AddToSave(transparentMaterial.ChangeColor);

        //transparentMaterial.SetActiveGameObject(false);

        //timDamaged.AddToUpdate(()=>transparentMaterial.ColorBlink(((int)(timDamaged.Percentage() * 10)) % 2 == 0)).AddToEnd(transparentMaterial.ColorBlinkEnd);

        return transparentMaterial;
    }

    private void UpdateSprite(SpriteRenderer sprite)
    {
        ((SpriteRenderer)originalSprite).sprite = sprite.sprite;
        ((SpriteRenderer)originalSprite).flipX = sprite.flipX;
        ((SpriteRenderer)originalSprite).color = sprite.color;
    }

    private void Health_death()
    {
        PoolManager.SpawnPoolObject(indexParticle, out onDeathParticle, transform.position, Quaternion.identity, null, false);

        var shape = onDeathParticle.shape;

        shape.sprite = ((SpriteRenderer)originalSprite).sprite;

        var aux = onDeathParticle.GetComponent<ParticleSystemRenderer>();

        aux.material.SetTexture("_MainTex", shape.sprite.texture);

        onDeathParticle.transform.up = transform.up;

        onDeathParticle.SetActiveGameObject(true);
    }

    private void Move_onMove(Vector2 obj)
    {
        if(obj.x < 0)
        {
            ((SpriteRenderer)originalSprite).flipX = defaultRight;
        }
        else if(obj.x > 0)
        {
            ((SpriteRenderer)originalSprite).flipX = !defaultRight;
        }
               
    }

    private void Entity_onDetected()
    {
        timDetected.Reset();
    }

    private void ChangeColor(Color save)
    {
        colorSetter.multiply = save;
    }

    private void ColorBlink(bool active)
    {
        if (active)
        {
            //parpadeo rapido
            colorSetter.Remove(damaged2);
            colorSetter.Add(damaged1);
        }
        else
        {
            //el mantenido
            colorSetter.Remove(damaged1);
            colorSetter.Add(damaged2);
        }
    }

    private void ColorBlinkEnd()
    {
        //volver al original
        colorSetter.Remove(damaged2);
        colorSetter.Remove(damaged1);
    }

    protected virtual void OnEnable()
    {
        if(isTransparent)
            EventManager.events.SearchOrCreate<EventGeneric>("move").action += UpdateTransparent;
    }

    private void OnDisable()
    {
        if (isTransparent)
            EventManager.events.SearchOrCreate<EventGeneric>("move").action -= UpdateTransparent;
    }

    private void UpdateTransparent(params object[] param)
    {
        if (!gameObject.activeSelf)
            return;

        Vector3 posPlayer = (Vector3)param[0];

        if (posPlayer.y > transform.position.y)
        {
            originalSprite.material.SetInt("_transparent", 1);
        }
        else
        {
            originalSprite.material.SetInt("_transparent", 0);
        }
    }

    private void ColorSetter_setter(Color obj)
    {
        ((SpriteRenderer)originalSprite).color = obj;
    }

    void ShakeSprite(Damage dmg)
    {
        if (_shakeDuration > 0 && gameObject.activeSelf)
        {
            shakeManager.Reset();
        }
    }

    void Shake()
    {
        Vector3 randomPoint = new Vector3(Random.Range(_initialPosition.x - _shakeIntensity, _initialPosition.x + _shakeIntensity), Random.Range(_initialPosition.y - _shakeIntensity, _initialPosition.y + _shakeIntensity), _initialPosition.z);
        transform.localPosition = randomPoint;
    }

    void EndShake()
    {
        transform.localPosition = _initialPosition;
    }
}


public class ComplexColor
{
    HashSet<Color> multiplyList = new HashSet<Color>();

    Color _multiply = Color.white;

    public Color multiply
    {
        get => _multiply;
        set
        {
            _multiply = value;

            RefreshColor();
        }
    }

    public event System.Action<Color> setter;

    void RefreshColor()
    {
        Color result = Color.white;
        foreach (var mul in multiplyList)
        {
            result *= mul;
        }

        setter?.Invoke(result * _multiply);
    }

    public void Add(Color c)
    {
        if (multiplyList.Add(c))
        {
            RefreshColor();
        }
    }

    public void Remove(Color c)
    {
        if (multiplyList.Remove(c))
        {
            RefreshColor();
        }
    }
}
