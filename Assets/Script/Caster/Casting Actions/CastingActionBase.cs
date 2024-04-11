using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CastingActionBase : ShowDetails
{
    public virtual CastingAction Create() 
    {
        var aux = System.Activator.CreateInstance(SetItemType()) as CastingAction;

        aux.castingActionBase = this;

        return aux;
    }

    protected abstract System.Type SetItemType();
}

public abstract class CastingAction : IAbilityComponent
{
    protected Ability ability;

    public CastingActionBase castingActionBase;

    public bool End { get => (ability).End; set => (ability).End = value; }

    public float FinalVelocity => (ability).FinalVelocity;

    public float FinalMaxRange => (ability).FinalMaxRange;

    public float FinalMinRange => (ability).FinalMinRange;

    public Vector3 Aiming => (ability).Aiming;

    public bool DontExecuteCast => (ability).DontExecuteCast;

    public bool onCooldownTime => (ability).onCooldownTime;

    public CasterEntityComponent caster => (ability).caster;

    public FadeColorAttack FeedBackReference { get => (ability).FeedBackReference; set => (ability).FeedBackReference = value; }

    public Timer cooldown => (ability).cooldown;

    public abstract IEnumerable<Entity> Cast(List<Entity> entities);

    public virtual void Init(Ability ability)
    {
        this.ability = ability;
    }

    public virtual void Destroy()
    {
        ability = null;
    }
}


public abstract class CastingAction<T>  : CastingAction where T: CastingActionBase
{
    new public T castingActionBase => (T)base.castingActionBase;
}

