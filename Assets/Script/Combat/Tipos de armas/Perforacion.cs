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
        // Implementación de la destrucción del arma
    }

    public override void Drop()
    {

    }

    public override void ButtomA()
    {
        // Implementación del botón A
    }

    public override void ButtomB()
    {
        // Implementación del botón B
    }

    public override void ButtomC()
    {
        // Implementación del botón C
    }

    #endregion
}
