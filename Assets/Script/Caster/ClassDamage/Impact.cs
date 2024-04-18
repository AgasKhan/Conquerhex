using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageTypes
{
    [CreateAssetMenu(menuName = "Weapons/Impact", fileName = "Impact")]
    /// <summary>
    /// Impactante - realiza mas danio a la vida en una relacion inversamente proporcional a la cantidad de regeneracion del enemigo
    /// </summary>
    public class Impact : PhysicalDamage
    {
        public override void IntarnalAction(Entity entity, float amount)
        {
            float multiply = entity.health.maxRegen > 0 ? 1 - (entity.health.actualRegen / entity.health.maxRegen) : 1;


            entity.TakeDamage(Damage.Create<PhysicalDamage>(multiply * amount));

            //entity.health.TakeLifeDamage(Random.Range(0, 0.5f) * amount);
        }
    }
}