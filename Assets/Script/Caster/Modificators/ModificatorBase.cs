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

        public virtual float FinalVelocity => original.FinalVelocity;

        public virtual float FinalMaxRange => original.FinalMaxRange;

        public virtual float FinalMinRange => original.FinalMinRange;

        public virtual int FinalMaxDetects => original.FinalMaxDetects;

        public virtual int MinDetects => original.MinDetects;

        public virtual float Angle => original.Angle;

        public virtual float Dot => original.Dot;
        public virtual float Auxiliar => original.Auxiliar;

        protected AbilityStats abilityModifier => modificatorBase.abilityModifier;

        protected OperationType operationType => modificatorBase.operationType;


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