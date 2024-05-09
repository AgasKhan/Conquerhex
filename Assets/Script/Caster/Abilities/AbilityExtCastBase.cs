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

    protected override Type SetItemType()
    {
        return typeof(AbilityExtCast);
    }
}

[System.Serializable]
public class AbilityExtCast : Ability
{
    CastingAction castingAction;

    public override bool DontExecuteCast => base.DontExecuteCast || castingAction ==null || castingAction.DontExecuteCast;

    protected override void Init()
    {
        base.Init();

        castingAction = ((AbilityExtCastBase)itemBase).castingAction.Create();

        castingAction.Init(this);
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
