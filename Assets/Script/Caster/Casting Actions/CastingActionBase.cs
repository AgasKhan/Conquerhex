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

    public Vector3 AimingXZ => (ability).AimingXZ;

    public Vector3 ObjectiveToAim { get => (ability).ObjectiveToAim; set => (ability).ObjectiveToAim = value; }

    public virtual bool DontExecuteCast => false;

    public bool onCooldownTime => (ability).onCooldownTime;

    public CasterEntityComponent caster => (ability).caster;

    public FadeColorAttack FeedBackReference { get => (ability).FeedBackReference; set => (ability).FeedBackReference = value; }

    public Timer cooldown => (ability).cooldown;

    public int FinalMaxDetects => (ability).FinalMaxDetects;

    public int MinDetects => (ability).MinDetects;

    public float Angle => (ability).Angle;

    public float Dot => (ability).Dot;

    public float Auxiliar => (ability).Auxiliar;

    /// <summary>
    /// Ejecuta el casteo personalizado de las AbilityExtCast
    /// </summary>
    /// <param name="entities"></param>
    /// <returns></returns>
    public abstract IEnumerable<Entity> InternalCastOfExternalCasting(List<Entity> entities, out bool showParticleInPos, out bool showParticleDamaged);

    public virtual void Init(Ability ability)
    {
        this.ability = ability;
    }

    public virtual CastingActionBase GetCastActionBase()
    {
        return castingActionBase;
    }

    public virtual void Destroy()
    {
        ability = null;
    }

    /// <summary>
    /// Al ser una accion de casteo debe de poseer la menor logica posible, ya que los triggers son los que preponderan la logica de trigger
    /// </summary>
    /// <param name="param"></param>
    public virtual void OnEnterState(CasterEntityComponent param)
    {
    }

    /// <summary>
    /// Al ser una accion de casteo debe de poseer la menor logica posible, ya que los triggers son los que preponderan la logica de trigger
    /// </summary>
    /// <param name="param"></param>

    public virtual void OnStayState(CasterEntityComponent param)
    {
    }

    /// <summary>
    /// Al ser una accion de casteo debe de poseer la menor logica posible, ya que los triggers son los que preponderan la logica de trigger
    /// </summary>
    /// <param name="param"></param>
    public virtual void OnExitState(CasterEntityComponent param)
    {
    }
}


public abstract class CastingAction<T>  : CastingAction where T: CastingActionBase
{
    new public T castingActionBase => (T)base.castingActionBase;
}

