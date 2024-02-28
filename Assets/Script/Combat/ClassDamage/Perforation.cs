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
            //entity.health.TakeRegenDamage();
            var aux = 3f / Random.Range(1, 4);

            entity.health.TakeRegenDamage(aux * amount);
        }
    }
}
