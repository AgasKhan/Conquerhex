using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AbilityModificators
{
    public abstract class ModificatorBase : ScriptableObject
    {
        [SerializeField]
        public AbilityStats abilityModifier;

        [SerializeField]
        public OperationType operationType;

        public virtual Modificator Create()
        {
            var aux = System.Activator.CreateInstance(SetItemType()) as Modificator;

            aux.modificatorBase = this;

            return aux;
        }

        protected abstract System.Type SetItemType();
    }


    public abstract class Modificator : IAbilityStats
    {
        public ModificatorBase modificatorBase;

        public IAbilityStats original;

        public virtual float FinalVelocity => abilityModifier.FinalVelocity == 0 ? original.FinalVelocity
        : Operation(original.FinalVelocity, abilityModifier.FinalVelocity);

        public virtual float FinalMaxRange => abilityModifier.FinalMaxRange == 0 ? original.FinalMaxRange
        : Operation(original.FinalMaxRange, abilityModifier.FinalMaxRange);

        public virtual float FinalMinRange => abilityModifier.FinalMinRange == 0 ? original.FinalMinRange
        : Operation(original.FinalMinRange, abilityModifier.FinalMinRange);

        public virtual int FinalMaxDetects => abilityModifier.FinalMaxDetects == 0 ? original.FinalMaxDetects
        : (int)Operation(original.FinalMaxDetects, abilityModifier.FinalMaxDetects);

        public virtual int MinDetects => abilityModifier.MinDetects == 0 ? original.MinDetects
        : (int) Operation(original.MinDetects, abilityModifier.MinDetects);

        public virtual float Angle => abilityModifier.Angle == 0 ? original.Angle
        : Mathf.Clamp(Operation(original.Angle, abilityModifier.Angle), 0, 360);

        public virtual float Dot => Utilitys.DotCalculate(Angle);

        public virtual float Auxiliar => abilityModifier.Auxiliar == 0 ? original.Auxiliar
        : Operation(original.Auxiliar, abilityModifier.Auxiliar);

        protected AbilityStats abilityModifier => modificatorBase.abilityModifier;

        protected OperationType operationType => modificatorBase.operationType;


        protected abstract float Operation(float previusValue, float flyweightValue);

        public virtual void OnEnterState(CasterEntityComponent param)
        {
        }

        public virtual void OnStayState(CasterEntityComponent param)
        {
        }

        public virtual void OnExitState(CasterEntityComponent param)
        {
        }

        public virtual void ControllerDown(Vector2 dir, float tim)
        {
        }

        public virtual void ControllerPressed(Vector2 dir, float tim)
        {
        }

        public virtual void ControllerUp(Vector2 dir, float tim)
        {
        }
    }


    public abstract class Modificator<T> : Modificator where T : ModificatorBase
    {
        new public T modificatorBase => (T)base.modificatorBase;

        
    }

    public enum OperationType
    {
        add,
        multiply
    }

    public enum TimeController
    {
        Down,
        Up
    }
}