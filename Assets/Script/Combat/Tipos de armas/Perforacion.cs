using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Perforacion : Weapon
{


    #region FUNCIONES 

    public override void Attack()
    {
        base.Attack();
    }

    public override void Durability()
    {
        durability -= 5;
        if (durability <= 0)
        {
            Destroy();
        }
    }

    public override void Destroy()
    {
        // Implementaci�n de la destrucci�n del arma
    }

    public override void Drop()
    {

    }

    public override void ButtomA()
    {
        // Implementaci�n del bot�n A
    }

    public override void ButtomB()
    {
        // Implementaci�n del bot�n B
    }

    public override void ButtomC()
    {
        // Implementaci�n del bot�n C
    }

    #endregion
}
