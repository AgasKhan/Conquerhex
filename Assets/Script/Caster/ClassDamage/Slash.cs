using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageTypes
{
    /// <summary>
    /// Cortante - realiza 0.1 de danio a la regeneracion por segundo, por la cantidad de danio % realizado a la vida
    /// </summary>
    [CreateAssetMenu(menuName = "Weapons/Slash", fileName = "Slash")]
    public class Slash : PhysicalDamage
    {
        public override void IntarnalAction(Entity entity, float amount)
        {
            if (entity.health.maxRegen <= 0)
                return;

            var dmg = Damage.Create<ElementalDamage>(Entity.tickTimeDamage/10);

            entity.Effect(amount / entity.health.maxLife * 100,
                () =>
                {
                    entity.TakeDamage(dmg);
                },
                null
                );
        }
    }
}