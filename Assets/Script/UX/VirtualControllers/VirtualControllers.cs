using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Controllers;

[CreateAssetMenu(menuName = "Controllers/VirtualControllers")]
public class VirtualControllers : SingletonScript<VirtualControllers>
{
    #region static classes

    static public HashSet<FatherKey> keys = new HashSet<FatherKey>();

    static public Axis Movement { get => instance._movement; }
    static public Axis Principal { get => instance._principal; }
    static public Axis Secondary { get => instance._secondary; }
    static public Axis Terciary { get => instance._terciary; }
    static public Axis Interact { get => instance._interact; }
    static public Axis Alpha1 { get => instance._alpha1; }
    static public Axis Alpha2 { get => instance._alpha2; }
    static public Axis Alpha3 { get => instance._alpha3; }
    static public Axis Alpha4 { get => instance._alpha4; }


    [SerializeField]
    Axis _movement;

    [SerializeField]
    Axis _principal;

    [SerializeField]
    Axis _secondary;

    [SerializeField]
    Axis _terciary;

    [SerializeField]
    Axis _interact;

    [SerializeField]
    Axis _alpha1;

    [SerializeField]
    Axis _alpha2;

    [SerializeField]
    Axis _alpha3;

    [SerializeField]
    Axis _alpha4;

    #endregion

    static public bool eneable
    {
        set
        {

            instance._eneable = value;
        }
        get
        {
            return instance._eneable;
        }
    }



    bool _eneable;


    #region unity functions

    public void MyAwake()
    {
        //_eneable = true;

        foreach (var item in keys)
        {
            item.MyAwake();
        }
    }

    public void MyDestroy()
    {
        foreach (var item in keys)
        {
            item.Destroy();
        }
    }

    #endregion
}


namespace Controllers
{
    #region class

    /// <summary>
    /// clase base de la deteccion de teclas
    /// </summary>
    public abstract class FatherKey : ShowDetails
    {
        public float timePressed;

        public bool enable = true;

        public abstract void Destroy();

        public abstract void MyAwake();
    }
    #endregion
}
