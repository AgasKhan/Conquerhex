using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    #region VARIABLES

    [SerializeField] float damage;
    [SerializeField] float durability;
    [SerializeField] float sinDefinir;

    #endregion

    #region FUNCIONES

    #region VIRTUALES
    public virtual void Attack()
    {

    }

    public virtual void Durability()
    {

    }

    #endregion

    #region ABSTRACTAS
    public abstract void Destroy();
    public abstract void ButtomA();
    public abstract void ButtomB();
    public abstract void ButtomC();

    #endregion

    #endregion

    #region UNITY FUNCIONES


    #endregion
}
