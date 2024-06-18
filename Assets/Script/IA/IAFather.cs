using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IAFather : MonoBehaviour, IState<Character>, IDamageable
{
    //public Timer timerStun = null;

    Character _character;

    Timer deathWait;

    public Character character => _character;

    public CasterEntityComponent caster => _character.caster;

    public EventControllerMediator moveEventMediator => character.moveEventMediator;

    public EventControllerMediator attackEventMediator => character.attackEventMediator;

    public EventControllerMediator dashEventMediator => character.dashEventMediator;

    public EventControllerMediator abilityEventMediator => character.abilityEventMediator;

    public event System.Action detect;

    public void Detect()
    {
        detect?.Invoke();
    }

    void Awake()
    {
        deathWait = TimersManager.Create(1, () => gameObject.SetActive(false)).Stop();
    }

    public void InternalTakeDamage(ref Damage dmg, int weightAction = 0 ,Vector3? damageOrigin = null)
    {
        if (dmg.amount <= 0)
            return;
    }

    public virtual void OnEnterState(Character param)
    {
        _character = param;

        param.health.death += Health_death;

        if (param.health.IsDeath)
        {
            Health_death();
            return;
        }
    }

    public virtual void OnStayState(Character param)
    {
    }

    public virtual void OnExitState(Character param)
    {
        param.health.death -= Health_death;

        _character = null;
    }

    protected virtual void Health_death()
    {
        character.CurrentState = null;
        deathWait?.Reset();
    }
}
