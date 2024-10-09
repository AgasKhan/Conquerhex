using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Ability with External Cast flyweight
/// </summary>
[CreateAssetMenu(menuName = "Abilities/Ability", fileName = "new Ability")]
public class AbilityExtCastBase : AbilityBase
{
    [Space]
    [SerializeField, Header("Accion a realizar cuando se ejecute el casteo")]
    public CastingActionBase castingAction;

    public override Type GetItemType()
    {
        return typeof(AbilityExtCast);
    }
}

[System.Serializable]
public class AbilityExtCast : Ability
{
    public CastingAction castingAction;

    public override bool DontExecuteCast => base.DontExecuteCast || castingAction ==null || castingAction.DontExecuteCast;

    protected override void Init()
    {
        base.Init();

        castingAction = ((AbilityExtCastBase)itemBase).castingAction.Create();

        castingAction.Init(this);
    }

    public override void OnEnterState(CasterEntityComponent param)
    {
        base.OnEnterState(param);
        castingAction.OnEnterState(param);
    }

    public override void OnStayState(CasterEntityComponent param)
    {
        base.OnStayState(param);
        castingAction.OnStayState(param);
    }

    public override void OnExitState(CasterEntityComponent param)
    {
        base.OnExitState(param);
        castingAction.OnExitState(param);
    }


    public override void Destroy()
    {
        castingAction.Destroy();

        base.Destroy();
    }

    protected override IEnumerable<Entity> InternalCast(List<Entity> entities, out bool showParticleInPos, out bool showParticleDamaged)
    {
        return castingAction.InternalCastOfExternalCasting(entities, out showParticleInPos, out showParticleDamaged);
    }
}
