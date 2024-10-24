using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Toxine", fileName = "Toxine")]
public class Toxine : ElementalDamage
{
    public override void IntarnalAction(Entity go, float amount)
    {
        go.Effect((1-(go.health.actualRegen / go.health.maxRegen)) *10, ()=> ToxineUpdate(go, amount), null);
    }

    void ToxineUpdate(Entity go, float amount)
    {
        go.health.TakeLifeDamage(amount/10 * Time.deltaTime);
    }
}
