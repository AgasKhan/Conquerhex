using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObjects : StaticEntity
{
    [SerializeField, Range (0f, 2f)]
    float _shakeIntensity;

    [SerializeField, Range(0.1f, 100f)]
    float _shakeFrecuency=10;


    [SerializeField, Range (0f, 2f)]
    float _shakeDuration;

    [SerializeField]
    StructureBase _structure;

    Vector3 _initialPosition;

    Timer shakeManager;

    protected override Damage[] vulnerabilities => _structure.vulnerabilities;

    protected override void Config()
    {
        base.Config();

        MyAwakes += MyAwake;
    }

    private void MyAwake()
    {
        LoadSystem.AddPostLoadCorutine(InitDestructibleObjs);
        
    }

    void InitDestructibleObjs()
    {
        _initialPosition = transform.position;

        onTakeDamage += ShakeSprite;

        health.noLife += Health_noLife;

        health.Init(_structure.life, _structure.regen);

        shakeManager = TimersManager.Create(_shakeDuration, Shake, EndShake).Stop();
    }

    private void Health_noLife()
    {
        gameObject.SetActive(false);
    }

    void ShakeSprite()
    {
        if(_shakeDuration > 0 && gameObject.activeSelf)
        {
            shakeManager.Reset();
        }
    }

    void Shake()
    {
        /*
        if ((int)(Time.realtimeSinceStartup * _shakeFrecuency) % 2 == 0)
            return;
        */
        Vector3 randomPoint = new Vector3(Random.Range(_initialPosition.x - _shakeIntensity, _initialPosition.x + _shakeIntensity), Random.Range(_initialPosition.y - _shakeIntensity, _initialPosition.y + _shakeIntensity), _initialPosition.z);
        transform.position = randomPoint;
    }

    void EndShake()
    {
        transform.position = _initialPosition;
    }

}
