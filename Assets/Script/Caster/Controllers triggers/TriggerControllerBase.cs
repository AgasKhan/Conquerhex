using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TriggerControllerBase : ShowDetails
{
    public virtual TriggerController Create()
    {
        var aux = System.Activator.CreateInstance(SetItemType()) as TriggerController;

        aux.triggerBase = this;

        return aux;
    }

    protected abstract System.Type SetItemType();
}

public abstract class TriggerController : IControllerDir, IAbilityComponent
{
    public TriggerControllerBase triggerBase;

    protected Ability ability;

    public bool End
    {
        get => ability.End;
        set
        {
            ability.End = value;
        }
    }

    public float FinalVelocity => ability.FinalVelocity;

    public float FinalMaxRange => ability.FinalMaxRange;

    public float FinalMinRange => ability.FinalMinRange;

    public float Angle => ability.Angle;

    public float Dot => ability.Dot;

    public bool DontExecuteCast => ability.DontExecuteCast;

    public bool onCooldownTime => ability.onCooldownTime;

    public CasterEntityComponent caster => ability.caster;
    public Timer cooldown => ability.cooldown;

    public List<Entity> affected => ability.affected;

    public FadeColorAttack FeedBackReference
    {
        get => ability.FeedBackReference;
        set
        {
            ability.FeedBackReference = value;
        }
    }

    public virtual Vector3 Aiming
    {
        get => ability.Aiming;

        set
        {
            ability.Aiming = value;
        }
    }

    public int FinalMaxDetects => throw new System.NotImplementedException();

    public int MinDetects => throw new System.NotImplementedException();

    public float Auxiliar => ((IAbilityStats)ability).Auxiliar;

    public void Cast() 
        => ability.Cast();


    public List<Entity> Detect(Entity caster, Vector3 pos)
        => ability.Detect(caster, pos);//tiene invertido el lugar de minRange y maxRange para mantener compatibilidad

    public List<Entity> Detect() 
        => ability.Detect(caster.container, caster.transform.position);//tiene invertido el lugar de minRange y maxRange para mantener compatibilidad

    public virtual void Init(Ability ability)
    {
        this.ability = ability;
    }

    public virtual void Set()
    {
    }

    public virtual void Destroy()
    {
        ability = null;
    }

    public virtual void OnEnterState(CasterEntityComponent param)
    {
        ability.End = false;
        ability.state = Ability.State.start;
        param.abilityControllerMediator += ability;
        ability.onCast += param.AttackEvent;
        ability.ControllerDown(Vector2.zero, 0);
    }

    public virtual void OnStayState(CasterEntityComponent param)
    {
    }

    public virtual void OnExitState(CasterEntityComponent param)
    {
        //Debug.Log("sali");
        //ability.StopCast();
        param.abilityControllerMediator -= ability;
        ability.onCast -= param.AttackEvent;
    }

    public abstract void ControllerDown(Vector2 dir, float tim);
    public abstract void ControllerPressed(Vector2 dir, float tim);
    public abstract void ControllerUp(Vector2 dir, float tim);

    public virtual List<Entity> InternalDetect(Entity caster, Vector3 pos, Vector3 dir, float timePressed = 0, float? minRange=null, float? maxRange=null, float? dot = null)
    {
        return ability.itemBase.Detect(ref ability.affected, caster, pos ,dir, ability.itemBase.FinalMaxDetects, minRange ?? FinalMinRange, maxRange ?? FinalMaxRange, dot ?? Dot);
    }
}

public interface IAbilityStats
{
    public float FinalVelocity { get; }

    public float FinalMaxRange { get; }

    public float FinalMinRange { get; }

    public int FinalMaxDetects { get; }

    public int MinDetects { get; }

    public float Angle { get; }

    public float Dot { get; }

    public float Auxiliar { get; }
}

public interface IAbilityComponent : IAbilityStats
{
    /// <summary>
    /// Variable dedicada a senializar el fin de la habilidad<br/>
    /// Criterio de utilizacion:<br/>
    /// Los TriggerControllers tienen prioridad sobre los casteos, en caso que se desee invalidar el control del casteo<br/>
    /// Los CastingActions deben de setearlo en true en algun lado de su codigo (las weaponsKata lo hacen de forma automatica al atacar)
    /// </summary>
    public bool End { get ; set ; }

    public  Vector3 Aiming { get ; }

    public  bool DontExecuteCast { get; }

    public bool onCooldownTime { get; }

    public FadeColorAttack FeedBackReference { get; set; }

    public Timer cooldown { get; }
}

//ModificadorContainer
//Modificador
//