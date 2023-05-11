using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticAttack
{
    public Timer timerToAttack;
    WeaponKata kata;

    public event System.Action onAttack;

    public bool cooldown => kata.cooldownTime;

    public float radius
    {
        get
        {
            return kata.itemBase.detect.radius;
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

        timerToAttack = TimersManager.Create(Random.Range(3, 7) / 3f, () => {

            actual = Color.Lerp(areaColor, attackColor, timerToAttack.InversePercentage());

        }, () =>
        {
            kata.ControllerUp(Vector2.zero, 0);

            onAttack?.Invoke();
        }).Stop();

        timerToAttack.current = 0;
    }
}
