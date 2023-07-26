using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticAttack
{
    public Timer timerToAttack;
    EquipedItem<WeaponKata> kata;

    public event System.Action onAttack;

    public bool cooldown => kata.equiped.cooldownTime && timerToAttack.Chck;

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
            if (kata.equiped.reference != null)
                return kata.equiped.reference.attackColor;
            else
                return Color.white;
        }
    }

    Color areaColor
    {
        get
        {
            if (kata.equiped.reference != null)
                return kata.equiped.reference.areaColor;
            else
                return Color.white;
        }
    }

    Color actual
    {
        get
        {
            if (kata.equiped.reference != null)
                return kata.equiped.reference.color;
            else
                return Color.white;
        }
        set
        {
            if (kata.equiped.reference != null)
                kata.equiped.reference.color = value;
        }
    }

    public void Attack()
    {
        kata.equiped.ControllerDown(Vector2.zero, 0);

        timerToAttack.Reset();
    }

    public AutomaticAttack(EquipedItem<WeaponKata> kata)
    {
        this.kata = kata;

        timerToAttack = null;

        timerToAttack = TimersManager.Create(2, () => {

            actual = Color.Lerp(areaColor, attackColor, timerToAttack.InversePercentage());

            this.kata.equiped.ControllerPressed(Vector2.zero, timerToAttack.total - timerToAttack.current);

        }, () =>
        {
            this.kata.equiped.ControllerUp(Vector2.zero, timerToAttack.total);

            onAttack?.Invoke();

        }).Stop();

        timerToAttack.current = 0;
    }
}
