using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDamageEntity : MonoBehaviour
{
    [Header("Damage config")]
    [SerializeField]
    Damage damage;

    [SerializeField]
    int damageWeight;

    [SerializeField]
    Vector3 offsetPosition;

    [SerializeField]
    float tickDamage = 1;

    [Header("FeedBack")]
    [SerializeField]
    new Renderer renderer;

    [SerializeField]
    Color notDamageColor;

    [SerializeField]
    Color damageColor;

    Timer timerDamage;

    Entity entity;    

    private void Awake()
    {
        timerDamage = TimersManager.Create(tickDamage, MyUpdateFeedBack, MyUpdateTick).SetLoop(true).Stop();
    }

    void MyUpdateTick()
    {
        entity.TakeDamage(damage, damageWeight, transform.position + offsetPosition);
    }

    void MyUpdateFeedBack()
    {
        renderer.material.color = Color.Lerp(notDamageColor, damageColor, (Mathf.RoundToInt(((timerDamage.InversePercentage() + timerDamage.total.RelativePercentage(0.3f)) *10)) / 3) * 3 / 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Entity>(out entity))
        {
            timerDamage.Reset();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Entity>(out entity))
        {
            timerDamage.Stop();
        }
    }

    public void StopTimer() => timerDamage.Stop();
}
