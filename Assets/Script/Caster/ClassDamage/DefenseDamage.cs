using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageTypes
{
    /// <summary>
    /// Defensa - Dania exclusivamente a la barra de defensa
    /// </summary>
    [CreateAssetMenu(menuName = "Weapons/Defense", fileName = "Defense")]
    public class DefenseDamage : PureDamage
    {
        public override void IntarnalAction(Entity entity, float amount)
        {
            
        }
    }
}

