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
    private void OnEnable()
    {
        _initialPosition = transform.position;

        health.noLife += ShakeSprite;
    }


    void ShakeSprite()
    {
        if(_shakeDuration > 0)
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
            Vector3 randomPoint = new Vector3(Random.Range(-1f, 1f) * _shakeIntensity, Random.Range(-1f, 1f) * _shakeIntensity, _initialPosition.z);
            transform.position = randomPoint;
            yield return null;
        }

        _pendingShake = 0f;
        transform.position = _initialPosition;

    }

}
