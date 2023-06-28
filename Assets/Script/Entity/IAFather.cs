using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAFather : MonoBehaviour, IState<Character>, IDamageable
{
    public TimedAction timerStun = null;

    protected Character character;

    public event System.Action onAttack;

    public event System.Action<Vector2> onMove;

    public event System.Action onIdle;


    void Awake()
    {
        timerStun = TimersManager.Create(0.33f, () =>
        {
            enabled = true;
        });
    }

    public void TakeDamage(Damage dmg)
    {
        if (dmg.amount <= 0)
            return;
        enabled = false;
        timerStun.Reset();
    }

    protected void AttackAnimation()
    {
        onAttack?.Invoke();
    }

    protected void IdleAnimation()
    {
        onIdle?.Invoke();
    }

    protected void MoveAnimation(Vector2 dir)
    {
        onMove?.Invoke(dir);
    }

    public abstract void OnEnterState(Character param);
    public abstract void OnExitState(Character param);
    public abstract void OnStayState(Character param);

    
}
