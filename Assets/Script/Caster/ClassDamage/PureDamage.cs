using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageTypes
{

    [CreateAssetMenu(menuName = "Weapons/PureDamage", fileName = "PureDamage")]
    /// <summary>
    /// Puro - padre de todos los tipos de danio - en su forma pura hace el danio de forma comun y corriente
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