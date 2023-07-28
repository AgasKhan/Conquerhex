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

    [SerializeField, Range(0f, 2f)]
    float _shakeIntensity;

    [SerializeField, Range(0f, 2f)]
    float _shakeDuration;

    Vector3 _initialPosition;

    Timer shakeManager;

    TimedCompleteAction timDamaged = null;

    TimedCompleteAction timDetected = null;



    protected virtual void Awake()
    {
        //originalSprite = GetComponentInChildren<Renderer>();
        originalSprite.material = transparentMaterial;

        originalSprite.sortingOrder = Mathf.RoundToInt(transform.position.y * -100);

        SpriteRenderer originalSpriteRenderer = null;

        _initialPosition = transform.localPosition;

        if (mainTexture!=null)
            originalSprite.material.SetTexture("_MainTex", mainTexture);

        if (!(originalSprite is SpriteRenderer))
            return;

        originalSpriteRenderer = (SpriteRenderer)originalSprite;

        colorSetter.setter += ColorSetter_setter;

        colorSetter.Add(originalSpriteRenderer.color);

        if (!transform.parent.TryGetComponent(out Entity entity))
            return;

        entity.onTakeDamage += ShakeSprite;

        entity.onDetected += Entity_onDetected;

        if(entity is DynamicEntity)
        {
            ((DynamicEntity)entity).move.onMove += Move_onMove;
        }

        shakeManager = TimersManager.Create(_shakeDuration, Shake, EndShake).Stop();

        timDetected = TimersManager.LerpInTime(detected, Color.white, 0.1f, Color.Lerp, (save) => colorSetter.multiply = save);

        timDamaged = TimersManager.Create(0.33f, () => {

            if (((int)(timDamaged.Percentage() * 10)) % 2 == 0)
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

        }, () => {

            //volver al original
            colorSetter.Remove(damaged2);
            colorSetter.Remove(damaged1);

        });

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

        setter(result * _multiply);
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
