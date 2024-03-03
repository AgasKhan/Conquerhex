using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageTypes
{

    [CreateAssetMenu(menuName = "Weapons/Slash", fileName = "Slash")]
    public class Slash : PhysicalDamage
    {
        public override void IntarnalAction(Entity entity, float amount)
        {
            if (entity.health.maxRegen <= 0)
                return;

            var dmg = Damage.Create<ElementalDamage>(1);

            entity.Effect(amount / 3,
                () =>
                {
                    //dmg.amount = 1;

                    entity.TakeDamage(dmg);
                },
                null
                );
        }
    }
}