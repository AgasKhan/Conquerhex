using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticAttack
{
    public TimedAction timerToAttack;
    public Timer timerChargeAttack;
    EquipedItem<WeaponKata> kata;

    public event System.Action onAttack;

    public bool cooldown => kata.equiped!=null ? kata.equiped.cooldownTime && timerChargeAttack.Chck : false;

    public WeaponKata weaponKata => kata.equiped;

    public float radius
    {
        get
        {
            return kata.equiped.finalRange;
        }
    }

    Color attackColor
    {
        get
        {
            if (weaponKata != null && weaponKata.reference != null)
                return kata.equiped.reference.attackColor;
            else
                return Color.white;
        }
    }

    Color areaColor
    {
        get
        {
            if (weaponKata != null && weaponKata.reference != null)
                return kata.equiped.reference.areaColor;
            else
                return Color.white;
        }
    }

    Color actual
    {
        get
        {
            if (weaponKata != null && weaponKata.reference != null)
                return kata.equiped.reference.color;
            else
                return Color.white;
        }
        set
        {
            if (weaponKata != null && weaponKata.reference != null)
                kata.equiped.reference.color = value;
        }
    }

    public void Attack()
    {
        if (kata.equiped != null)
            kata.equiped.ControllerDown(Vector2.zero, 0);

        timerChargeAttack.Reset();
    }

    public AutomaticAttack(EquipedItem<WeaponKata> kata)
    {
        this.kata = kata;

        timerToAttack = (TimedAction)TimersManager.Create(2, Attack).Stop().SetLoop(true);

        timerChargeAttack = null;

        timerChargeAttack = TimersManager.Create(2, () => {

            actual = Color.Lerp(areaColor, attackColor, timerChargeAttack.InversePercentage());

            if (this.kata != null && this.kata.equiped != null)
                this.kata.equiped.ControllerPressed(Vector2.zero, timerChargeAttack.total - timerChargeAttack.current);

        }, () =>
        {
            if (this.kata != null && this.kata.equiped != null)
            {
                this.kata.equiped.ControllerUp(Vector2.zero, timerChargeAttack.total);   
            }
 
            onAttack?.Invoke();

        }).Stop().SetInitCurrent(0);
        
    }
}
