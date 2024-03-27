using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageTypes
{
    [CreateAssetMenu(menuName = "Weapons/Perforation", fileName = "Perforation")]
    public class Perforation : PhysicalDamage
    {
        public override void IntarnalAction(Entity entity, float amount)
        {
            if (entity.health.maxRegen <= 0)
                return;

            //entity.health.TakeRegenDamage();
            var aux =  Random.Range(1, 4) / 3f;

            Damage damage = Damage.Create<ElementalDamage>(aux * amount);

            entity.Effect(0, null, () => 
            {
                entity.TakeDamage(damage);
            });
        }
    }
}
