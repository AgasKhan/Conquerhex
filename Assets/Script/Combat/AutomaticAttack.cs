using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticAttack
{
    public Timer timerToAttack;
    WeaponKata kata;

    public event System.Action onAttack;

    public bool cooldown => kata.cooldownTime && timerToAttack.Chck;

    public float radius
    {
        get
        {
            return kata.finalRange;
        }
    }

    Color attackColor
    {
        get
        {
            if (kata.reference != null)
                return kata.reference.attackColor;
            else
                return Color.white;
        }
    }

    Color areaColor
    {
        get
        {
            if (kata.reference != null)
                return kata.reference.areaColor;
            else
                return Color.white;
        }
    }

    Color actual
    {
        get
        {
            if (kata.reference != null)
                return kata.reference.color;
            else
                return Color.white;
        }
        set
        {
            if (kata.reference != null)
                kata.reference.color = value;
        }
    }

    public void Attack()
    {
        kata.ControllerDown(Vector2.zero, 0);

        timerToAttack.Reset();
    }

    public AutomaticAttack(WeaponKata kata)
    {
        this.kata = kata;

        timerToAttack = null;

        timerToAttack = TimersManager.Create(2, () => {

            actual = Color.Lerp(areaColor, attackColor, timerToAttack.InversePercentage());

            this.kata.ControllerPressed(Vector2.zero, timerToAttack.total - timerToAttack.current);

        }, () =>
        {
            this.kata.ControllerUp(Vector2.zero, timerToAttack.total);

            onAttack?.Invoke();
        }).Stop();

        timerToAttack.current = 0;
    }
}
