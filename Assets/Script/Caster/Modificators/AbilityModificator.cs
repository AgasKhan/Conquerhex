using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilityModificators;

//Lo tiene la habilidad
public class AbilityModificator : IAbilityStats
{
    protected Ability ability;

    Modificator[] modificators;

    public float FinalVelocity => modificators == null ? ability.itemBase.FinalVelocity : modificators[modificators.Length-1].FinalVelocity;

    public float FinalMaxRange => modificators == null? ability.itemBase.FinalMaxRange : modificators[modificators.Length - 1].FinalMaxRange;

    public float FinalMinRange => modificators == null? ability.itemBase.FinalMinRange : modificators[modificators.Length - 1].FinalMinRange;

    public int FinalMaxDetects => modificators == null ? ability.itemBase.FinalMaxDetects : modificators[modificators.Length - 1].FinalMaxDetects;

    public int MinDetects => modificators == null ? ability.itemBase.MinDetects : modificators[modificators.Length - 1].MinDetects;

    public float Angle => modificators == null  ? ability.itemBase.Angle : modificators[modificators.Length - 1].Angle;

    public float Dot => modificators == null ? ability.itemBase.Dot : modificators[modificators.Length - 1].Dot;

    public float Auxiliar => modificators == null ? ability.itemBase.Auxiliar : modificators[modificators.Length - 1].Auxiliar;

    #region mediator

    public void OnEnterState(CasterEntityComponent param)
    {
        if (modificators == null)
            return;
        for (int i = 0; i < modificators.Length; i++)
        {
            modificators[i].OnEnterState(param);
        }
    }

    public void OnStayState(CasterEntityComponent param)
    {
        if (modificators == null)
            return;
        for (int i = 0; i < modificators.Length; i++)
        {
            modificators[i].OnStayState(param);
        }
    }

    public void OnExitState(CasterEntityComponent param)
    {
        if (modificators == null)
            return;
        for (int i = 0; i < modificators.Length; i++)
        {
            modificators[i].OnExitState(param);
        }
    }

    public void ControllerDown(Vector2 dir, float tim)
    {
        if (modificators == null)
            return;
        for (int i = 0; i < modificators.Length; i++)
        {
            modificators[i].ControllerDown(dir, tim);
        }
    }

    public void ControllerPressed(Vector2 dir, float tim)
    {
        if (modificators == null)
            return;
        for (int i = 0; i < modificators.Length; i++)
        {
            modificators[i].ControllerPressed(dir, tim);
        }
    }

    public void ControllerUp(Vector2 dir, float tim)
    {
        if (modificators == null)
            return;
        for (int i = 0; i < modificators.Length; i++)
        {
            modificators[i].ControllerUp(dir, tim);
        }
    }

    #endregion

    public void Init(Ability ability)
    {
        this.ability = ability;

        if (ability.itemBase.modificators == null || ability.itemBase.modificators.Length <= 0)
            return;

        Debug.Log($"posee modificores: en la habilidad {ability.itemBase.nameDisplay} la cantidad de: {ability.itemBase.modificators.Length}");

        modificators = new Modificator[ability.itemBase.modificators.Length];

        modificators[0] = ability.itemBase.modificators[0].Create();
        modificators[0].original = ability.itemBase;

        for (int i = 1; i < ability.itemBase.modificators.Length; i++)
        {
            modificators[i] = ability.itemBase.modificators[i].Create();
            modificators[i].original = modificators[i-1];
        }
    }

    public virtual void Destroy()
    {
        ability = null;
    }


}





