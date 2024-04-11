using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageTypes
{
    /// <summary>
    /// Frio - El daño que haces originalmente a la vida, es el que le quitas a la cantidad máxima de vida en la barra 
    /// </summary>
    [CreateAssetMenu(menuName = "Weapons/Cold", fileName = "Cold")]
    public class Cold : ElementalDamage
    {
        public override void IntarnalAction(Entity go, float amount)
        {
            go.TakeDamage(Damage.Create<DebuffLife>(amount));
        }
    }
}




