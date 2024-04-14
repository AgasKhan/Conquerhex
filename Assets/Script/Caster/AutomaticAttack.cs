using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AutomaticAttack
{
    public TimedAction timerToAttack;
    public Timer timerChargeAttack;

    CasterEntityComponent owner;

    int indexKata;

    SlotItem<WeaponKata> kata => owner.katasCombo[indexKata];

    public event System.Action onAttack;

    public bool cooldown => kata.equiped!=null ? kata.equiped.onCooldownTime && timerChargeAttack.Chck : false;

    public WeaponKata weaponKata => kata.equiped;

    public float radius
    {
        get
        {
            if (weaponKata != null)
                return weaponKata.AttackArea;
            else
                return 0;
        }
    }

    Color attackColor
    {
        get
        {
            if (weaponKata != null && weaponKata.FeedBackReference != null)
                return kata.equiped.FeedBackReference.attackColor;
            else
                return Color.white;
        }
    }

    Color areaColor
    {
        get
        {
            if (weaponKata != null && weaponKata.FeedBackReference != null)
                return kata.equiped.FeedBackReference.areaColor;
            else
                return Color.white;
        }
    }

    Color actual
    {
        get
        {
            if (weaponKata != null && weaponKata.FeedBackReference != null)
                return kata.equiped.FeedBackReference.color;
            else
                return Color.white;
        }
        set
        {
            if (weaponKata != null && weaponKata.FeedBackReference != null)
                kata.equiped.FeedBackReference.color = value;
        }
    }

    public void Attack()
    {
        if (kata==null || weaponKata == null || timerChargeAttack == null || !timerChargeAttack.Chck)
            return;

        weaponKata.ControllerDown(Vector2.zero, 0);

        timerChargeAttack.Reset();
    }

    public void StopTimers()
    {
        weaponKata?.ControllerUp(Vector2.zero, timerChargeAttack.total);
        timerChargeAttack.Stop().SetInitCurrent(0);
        timerToAttack.Set(1).Stop();
    }

    public AutomaticAttack(CasterEntityComponent entity, int index)
    {
        owner = entity;
        indexKata = index;

        timerToAttack = (TimedAction)TimersManager.Create(1, Attack).Stop().SetLoop(true);

        timerChargeAttack = null;

        timerChargeAttack = TimersManager.Create(2, () => {

            actual = Color.Lerp(areaColor, attackColor, timerChargeAttack.InversePercentage());

            weaponKata?.ControllerPressed(Vector2.zero, timerChargeAttack.total - timerChargeAttack.current);

        }, () =>
        {
            
            weaponKata?.ControllerUp(Vector2.zero, timerChargeAttack.total);   
            
            onAttack?.Invoke();

        }).Stop().SetInitCurrent(0);
        
    }
}