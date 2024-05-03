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
