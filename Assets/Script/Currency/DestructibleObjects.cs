using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObjects : StaticEntity
{
    [SerializeField, Range (0f, 2f)]
    float _shakeIntensity;

    [SerializeField, Range (0f, 2f)]
    float _shakeDuration;

    [SerializeField]
    StructureBase _structure;


    float _pendingShake;
    Vector3 _initialPosition;


    protected override Damage[] vulnerabilities => _structure.vulnerabilities;

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyOnEnable;
    }


    private void MyOnEnable()
    {
        _initialPosition = transform.position;

        onTakeDamage += ShakeSprite;

        health.noLife += Health_noLife;

        health.Init(_structure.life, _structure.regen);
    }

    private void Health_noLife()
    {
        gameObject.SetActive(false);
    }

    void ShakeSprite()
    {
        if(_shakeDuration > 0 && gameObject.activeSelf)
        {
            _pendingShake += _shakeDuration;
            StartCoroutine(Shake());
        }
    }

    IEnumerator Shake()
    {
        var startTime = Time.realtimeSinceStartup;
        while (Time.realtimeSinceStartup < startTime + _pendingShake)
        {
            Vector3 randomPoint = new Vector3(Random.Range(_initialPosition.x - _shakeIntensity, _initialPosition.x + _shakeIntensity), Random.Range(_initialPosition.y - _shakeIntensity, _initialPosition.y + _shakeIntensity), _initialPosition.z);
            transform.position = randomPoint;
            yield return null;
        }

        _pendingShake = 0f;
        transform.position = _initialPosition;

    }

}
