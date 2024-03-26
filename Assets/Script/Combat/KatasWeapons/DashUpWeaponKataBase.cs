using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/DashUpWeaponKataBase")]
public class DashUpWeaponKataBase : WeaponKataBase
{
    protected override System.Type SetItemType()
    {
        return typeof(DashUpWeaponKata);
    }
}

public class DashUpWeaponKata : UpWeaponKata
{
    Timer timerToEnd;
    bool buttonPress;
    protected override void Init()
    {
        base.Init();
        timerToEnd = TimersManager.Create(1, () => End = true).Stop();
    }

    protected override void InternalControllerDown(Vector2 dir, float button)
    {
        buttonPress = true;
        base.InternalControllerDown(dir, button);
    }

    protected override void InternalControllerUp(Vector2 dir, float button)
    {
        if (!cooldown.Chck)
            return;

        cooldown.Reset();

        if (affected != null && affected.Count != 0 && caster.TryGetComponent<MoveEntityComponent>(out var aux))
        {
            aux.move.Velocity((affected[0].transform.position - caster.transform.position).normalized * itemBase.velocityCharge);
        }

        //Attack();

        //reference?.Attack();

        if (affected.Count == 0)
        {
            End = true;
            return;
        }

        timerToEnd.Reset();
        buttonPress = false;
    }

    public override void OnStayState(CasterEntityComponent param)
    {
        if (buttonPress)
            return;

        FeedBackReference.Area(originalScale * FinalRange * 1f / 4);
        Detect(Aiming, 0, FinalRange * 1f / 4);

        if (affected.Count == 0)
            return;

        Attack();

        FeedBackReference?.Attack();

        timerToEnd.Stop();

        End = true;
    }
}
