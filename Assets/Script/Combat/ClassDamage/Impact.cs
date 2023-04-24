using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Weapons/Impact", fileName = "Impact")]
/// <summary>
/// danio extra aleatorio de hasta el 50%
/// </summary>
public class Impact : PhysicalDamage
{
    public override void IntarnalAction(Entity entity, float amount)
    {
        entity.health.TakeLifeDamage(Random.Range(0, 0.5f) * amount);
    }
}