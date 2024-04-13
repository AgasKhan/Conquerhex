using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ViewEntityController : MonoBehaviour, ViewObjectModel.IViewController
{
    public Entity entity;

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

    [SerializeField]
    GameObject onDeathParticlePrefab;

    Vector2Int indexParticle;

    ParticleSystem onDeathParticle;

    Vector3 _initialPosition;

    TimedCompleteAction shakeManager;

    TimedCompleteAction timDamaged = null;

    TimedLerp<Color> timDetected = null;

    ViewObjectModel viewObjectModel;

    SpriteRenderer originalSpriteRenderer => (SpriteRenderer)viewObjectModel.originalRender;

    public void OnEnterState(ViewObjectModel param)
    {
        if (!transform.parent.TryGetComponent(out entity))
        {
            return;
        }

        viewObjectModel = param;

        _initialPosition = transform.localPosition;

        colorSetter.setter += ColorSetter_setter;

        colorSetter.Add(originalSpriteRenderer.color);

        entity.onTakeDamage += ShakeSprite;

        entity.onDetected += Entity_onDetected;

        shakeManager = (TimedCompleteAction)TimersManager.Create(_shakeDuration, Shake, EndShake).Stop();

        timDetected = TimersManager.Create(detected, Color.white, 0.1f, Color.Lerp, ChangeColor);

        timDamaged = TimersManager.Create(0.33f, () => ColorBlink(((int)(timDamaged.Percentage() * 10)) % 2 == 0), ColorBlinkEnd);

        if (entity.TryGetInContainer<MoveEntityComponent>(out var move))
        {
            move.onMove += Move_onMove;
        }

        if (onDeathParticlePrefab != null)
        {
            indexParticle = PoolManager.SrchInCategory("Particles", onDeathParticlePrefab.name);
            entity.health.death += Health_death;
        }
    }

    public void OnStayState(ViewObjectModel param)
    {
        throw new System.NotImplementedException();
    }

    public void OnExitState(ViewObjectModel param)
    {
        throw new System.NotImplementedException();
    }

    private void Move_onMove(Vector2 obj)
    {
        if (obj.x < 0)
        {
            originalSpriteRenderer.flipX = viewObjectModel.defaultRight;
        }
        else if (obj.x > 0)
        {
            originalSpriteRenderer.flipX = !viewObjectModel.defaultRight;
        }
    }

    private void Health_death()
    {
        PoolManager.SpawnPoolObject(indexParticle, out onDeathParticle, transform.position, Quaternion.identity, null, false);

        var shape = onDeathParticle.shape;

        shape.sprite = originalSpriteRenderer.sprite;

        var aux = onDeathParticle.GetComponent<ParticleSystemRenderer>();

        aux.material.SetTexture("_MainTex", shape.sprite.texture);

        onDeathParticle.transform.up = transform.up;

        onDeathParticle.SetActiveGameObject(true);
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
    private void ColorSetter_setter(Color obj)
    {
        originalSpriteRenderer.material.SetColor("_Color",obj);
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
