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
            entity.Effect(amount / 3,
                () =>
                {
                    entity.health.TakeRegenDamage(Time.deltaTime);
                },
                null
                );
        }
    }
}