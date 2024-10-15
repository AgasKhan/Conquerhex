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

    public Renderer Renderer => renderer;

    [SerializeField]
    Color notDamageColor;

    [SerializeField]
    Color damageColor;

    Timer timerDamage;

    public Entity entity;

    public System.Action UpdateFeedback, UpdateTick, ExitAction;

    public void Init(System.Action update = null, System.Action tick = null)
    {
        UpdateFeedback = update;

        UpdateFeedback ??= MyUpdateFeedBack;

        UpdateTick = tick;
        UpdateTick += MyUpdateTick;
        timerDamage = TimersManager.Create(tickDamage, UpdateFeedback, UpdateTick).SetLoop(true).Stop();
    }

    void MyUpdateTick()
    {
        entity?.TakeDamage(damage, damageWeight, transform.position + offsetPosition);
    }

    void MyUpdateFeedBack()
    {
        renderer.material.color = Color.Lerp(notDamageColor, damageColor, (Mathf.RoundToInt(((timerDamage.InversePercentage() + timerDamage.total.RelativePercentage(0.3f)) * 10)) / 3) * 3 / 10f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 15)
        {
            timerDamage?.Reset();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 15)
        {
            timerDamage?.Stop();
            ExitAction?.Invoke();
        }
    }

    public void StopTimer() => timerDamage.Stop();

    void OnDisable()
    {
        UpdateFeedback = null;
        UpdateTick = null;
    }
}
