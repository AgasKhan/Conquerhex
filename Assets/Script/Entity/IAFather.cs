using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAFather : MonoBehaviour, IState<Character>, IDamageable
{
    public Timer timerStun = null;

    protected Character character;

    void Awake()
    {
        timerStun = TimersManager.Create(0.33f, () =>
        {
            enabled = true;
        }, false);
    }

    public void TakeDamage(Damage dmg)
    {
        if (dmg.amount <= 0)
            return;
        enabled = false;
        timerStun.Reset();
    }

    public abstract void OnEnterState(Character param);
    public abstract void OnExitState(Character param);
    public abstract void OnStayState(Character param);

    
}
