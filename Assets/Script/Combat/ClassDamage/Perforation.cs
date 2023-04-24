using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Perforation", fileName = "Perforation")]
public class Perforation : PhysicalDamage
{
    public override void IntarnalAction(Entity entity, float amount)
    {
        //entity.health.TakeRegenDamage();
        var aux = 3 / Random.Range(1, 4);

        entity.health.TakeRegenDamage(aux * amount);
    }
}
