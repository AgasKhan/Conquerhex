using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageTypes
{

    [CreateAssetMenu(menuName = "Weapons/PureDamage", fileName = "PureDamage")]
    /// <summary>
    /// Clase padre de los tipos de danio
    /// </summary>
    public class PureDamage : ShowDetails
    {
        public Color color;

        public bool IsParent = false;

        public Target target = Target.all;

        public override string nameDisplay => this.GetType().Name;

        public virtual void IntarnalAction(Entity go, float amount)
        {
        }
    }

}