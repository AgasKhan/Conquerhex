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

    public event System.Action onAttack
    {
        add
        {
            weaponKata.onCast += value;
        }
        remove
        {
            weaponKata.onCast -= value;
        }
    }

    public bool cooldown => kata.equiped!=null ? kata.equiped.onCooldownTime && timerChargeAttack.Chck : false;

    public WeaponKata weaponKata => kata.equiped;

    FadeColorAttack FeedBackReference
    {
        get
        {
            if (weaponKata != null && weaponKata.FeedBackReference != null)
            {
                var aux = kata.equiped.FeedBackReference;
                aux.Area(radius);
                return aux;
            }

            return null;
        }
    }

    public float radius
    {
        get
        {
            if (weaponKata != null)
                return weaponKata.FinalMaxRange;
            else
                return 0;
        }
    }

    Color attackColor
    {
        get
        {
            if (FeedBackReference != null)
            {
                return FeedBackReference.attackColor;
            } 
            else
                return Color.white;
        }
    }

    Color areaColor
    {
        get
        {
            if (FeedBackReference != null)
            {
                return FeedBackReference.areaColor;
            } 
            else
                return Color.white;
        }
    }

    Color actual
    {
        get
        {
            if (FeedBackReference != null)
            {
                return FeedBackReference.color;
            }
            else
                return Color.white;
        }
        set
        {
            if (FeedBackReference != null)
                FeedBackReference.color = value.ChangeAlphaCopy(FeedBackReference.color.a);
        }
    }

    public void Attack()
    {
        if (kata==null || weaponKata == null || timerChargeAttack == null || !timerChargeAttack.Chck)
            return;

        weaponKata.ControllerDown(Vector2.zero, 0);

        timerChargeAttack.Reset();
    }

    public void ResetAttack()
    {
        if (!timerChargeAttack.Chck || !timerToAttack.Chck)
            return;
        
        timerChargeAttack.Stop();

        timerToAttack.Reset();
    }

    public void StopTimers()
    {
        Debug.Log("se stopeo el timer");
        weaponKata?.ControllerUp(Vector2.zero, timerChargeAttack.total);
        weaponKata?.StopCast();
        timerChargeAttack.Stop().SetInitCurrent(0);
        timerToAttack.Set(1).Stop().SetInitCurrent(0);
    }

    public AutomaticAttack(CasterEntityComponent entity, int index)
    {
        owner = entity;
        indexKata = index;

        timerToAttack = (TimedAction)TimersManager.Create(1, ()=>
        {
            actual = Color.Lerp(areaColor, attackColor, timerToAttack.InversePercentage());

        },Attack).Stop().SetInitCurrent(0);

        timerChargeAttack = null;

        timerChargeAttack = TimersManager.Create(2, () => 
        {

            weaponKata?.ControllerPressed(Vector2.zero, timerChargeAttack.total - timerChargeAttack.current);

        }, () =>
        {
            weaponKata?.ControllerUp(Vector2.zero, timerChargeAttack.total);   

            weaponKata.FeedBackReference=null;

        }).Stop().SetInitCurrent(0);
        
    }
}