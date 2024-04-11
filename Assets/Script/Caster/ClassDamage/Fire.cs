using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageTypes
{
    /// <summary>
    /// Calor- Te quita la mitad de la vida que te falta tanto en la vida como en la regeneracion
    /// </summary>
    [CreateAssetMenu(menuName = "Weapons/Fire", fileName = "Fire")]
    public class Fire : ElementalDamage
    {
        public override void IntarnalAction(Entity go, float amount)
        {
            float multiply = (1- (go.health.actualLife / go.health.maxLife)) * 1f/2;

            float multiply2 =(1- (go.health.actualRegen / go.health.maxRegen)) * 1f / 2;

            go.TakeDamage(Damage.Create<DebuffLife>(multiply * go.health.maxLife));

            go.TakeDamage(Damage.Create<DebuffRegen>(multiply2 * go.health.maxRegen));
        }
    }
}

