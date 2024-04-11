using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageTypes
{
    /// <summary>
    /// Perforante - realiza mas danio a la regeneracion en una realacion inversamente proporcional a la cantidad de salud del enemigo (1% minimo)
    /// </summary>
    [CreateAssetMenu(menuName = "Weapons/Perforation", fileName = "Perforation")]
    public class Perforation : PhysicalDamage
    {
        public override void IntarnalAction(Entity entity, float amount)
        {
            if (entity.health.maxRegen <= 0)
                return;

            float multiply = 1 - (entity.health.actualLife / entity.health.maxLife);

            if (multiply < 0.01f)
                multiply = 0.01f;

            Damage damage = Damage.Create<ElementalDamage>(multiply * amount);

            entity.Effect(0, null, () => 
            {
                entity.TakeDamage(damage);
            });
        }
    }
}
