using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TriggerControllerBase : ShowDetails
{
    public virtual TriggerController Create()
    {
        var aux = System.Activator.CreateInstance(SetItemType()) as TriggerController;

        aux.triggersBase = this;

        return aux;
    }

    protected abstract System.Type SetItemType();

}

public abstract class TriggerController : IControllerDir
{
    public TriggerControllerBase triggersBase;

    protected Ability ability;

    protected bool End { get => ability.End; set => ability.End = value; }

    public virtual float FinalVelocity => ability.FinalVelocity;

    public virtual float FinalRange => ability.FinalRange;

    public virtual Vector3 Aiming => ability.Aiming;

    public virtual bool DontExecuteCast => ability.DontExecuteCast;

    public bool onCooldownTime => ability.onCooldownTime;

    public CasterEntityComponent caster => ability.caster;

    public FadeColorAttack FeedBackReference
    {
        get => ability.FeedBackReference;
        set
        {
            ability.FeedBackReference = value;
        }
    }

    public void Cast() => ability.Cast();

    public Timer cooldown => ability.cooldown;

    public List<Entity> affected => ability.affected;

    public virtual void Init(Ability ability)
    {
        this.ability = ability;
    }

    public virtual void Set()
    {
    }

    public virtual void OnEnterState(CasterEntityComponent param)
    {
        End = false;
        param.attack += this;
        ControllerDown(ability.Aiming, 0);
    }

    public virtual void OnStayState(CasterEntityComponent param)
    {
    }

    public virtual void OnExitState(CasterEntityComponent param)
    {
        Debug.Log("sali");
        ability.StopCast();
        param.attack -= this;
    }

    public virtual List<Entity> Detect(Vector2 dir, float timePressed = 0, float? range = null, float? dot = null)
    {
        return ability.itemBase.Detect(ref ability.affected, ability.caster.container, dir, ability.itemBase.maxDetects, range ?? ability.FinalRange, dot ?? ability.itemBase.dot);
    }

    public abstract void ControllerDown(Vector2 dir, float tim);
    public abstract void ControllerPressed(Vector2 dir, float tim);
    public abstract void ControllerUp(Vector2 dir, float tim);
}

