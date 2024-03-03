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
            var dmg = Damage.Create<ElementalDamage>(0);

            entity.Effect(amount / 3,
                () =>
                {
                    dmg.amount = Time.deltaTime;

                    entity.TakeDamage(dmg);
                },
                null
                );
        }
    }
}