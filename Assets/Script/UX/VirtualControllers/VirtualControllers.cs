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

    static public HashSet<TriggerDetection> triggers = new HashSet<TriggerDetection>();

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

    TriggerDetection[] triggersArray;

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

    public void MyUpdate()
    {
        if (!eneable)
            return;

        foreach (var item in triggersArray)
        {
            item.Update();
        }
    }

    public void MyDestroy()
    {
        foreach (var item in keys)
        {
            item.Destroy();
        }
    }

    public void MyAwake()
    {
        //_eneable = true;

        foreach (var item in keys)
        {
            item.MyAwake();
        }

        if (triggersArray == null || triggersArray.Length < triggers.Count)
            triggersArray = new TriggerDetection[triggers.Count];

        triggers.CopyTo(triggersArray, 0);
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

        protected override void MyEnable()
        {
            VirtualControllers.keys.Add(this);
        }

        public abstract void Destroy();

        public abstract void MyAwake();
    }

    public abstract class TriggerDetection : ScriptableObject
    {

        [SerializeField, Tooltip("En caso de estar activo, se agregara a la lista de los triggers a detectar, en caso que no, no se ejecutara de forma automatica")]
        bool active = true;

        private void OnEnable()
        {
            if(active)
                VirtualControllers.triggers.Add(this);
        }

        public abstract void Update();
    }
    #endregion
}
