using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageTypes
{
    [CreateAssetMenu(menuName = "Weapons/Impact", fileName = "Impact")]
    /// <summary>
    /// danio extra puro aleatorio de hasta el 50%
    /// </summary>
    public class Impact : PhysicalDamage
    {
        public override void IntarnalAction(Entity entity, float amount)
        {
            


            entity.TakeDamage(Damage.Create<PhysicalDamage>(Random.Range(0 , 3) * 25 / 100f * amount));

            //entity.health.TakeLifeDamage(Random.Range(0, 0.5f) * amount);
        }
    }
}