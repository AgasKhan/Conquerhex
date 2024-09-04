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
    static public ButtonAxis Movement { get => instance._movement; }
    static public ButtonAxis Camera { get => instance._camera; }
    static public ButtonAxis Principal { get => instance._principal; }
    static public ButtonAxis Secondary { get => instance._secondary; }
    static public ButtonAxis Terciary { get => instance._terciary; }
    static public ButtonAxis Interact { get => instance._interact; }
    static public ButtonAxis Alpha1 { get => instance._alpha1; }
    static public ButtonAxis Alpha2 { get => instance._alpha2; }
    static public ButtonAxis Alpha3 { get => instance._alpha3; }
    static public ButtonAxis Alpha4 { get => instance._alpha4; }
    static public ButtonAxis Escape { get => instance._escape; }
    static public ButtonAxis Inventory { get => instance._inventory; }
    static public ButtonAxis Accept { get => instance._accept; }


    [SerializeField]
    ButtonAxis _movement;

    [SerializeField]
    ButtonAxis _camera;

    [SerializeField]
    ButtonAxis _principal;

    [SerializeField]
    ButtonAxis _secondary;

    [SerializeField]
    ButtonAxis _terciary;

    [SerializeField]
    ButtonAxis _interact;

    [SerializeField]
    ButtonAxis _alpha1;

    [SerializeField]
    ButtonAxis _alpha2;

    [SerializeField]
    ButtonAxis _alpha3;

    [SerializeField]
    ButtonAxis _alpha4;

    [SerializeField]
    ButtonAxis _escape;

    [SerializeField]
    ButtonAxis _accept;

    [SerializeField]
    ButtonAxis _inventory;

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

    public void DisableExceptTab()
    {
        foreach (var item in keys)
        {
            if (item != _inventory)
                item.enable = false;
        }
    }
    public void EnableAll()
    {
        foreach (var item in keys)
        {
            item.enable = true;
        }
    }

    #region unity functions

    public void MyUpdate()
    {
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
            item.enable = true;
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

        [SerializeField]
        protected ButtonAxis axis;

        protected bool press;

        [SerializeField]
        protected Vector2 dir;

        /*
        [SerializeField, Tooltip("En caso de estar activo, se agregara a la lista de los triggers a detectar, en caso que no, no se ejecutara de forma automatica")]
        bool active = true;
        */
        protected virtual void OnEnable()
        {
            //if(active)
                VirtualControllers.triggers.Add(this);
        }

        public abstract void Update();
    }
    #endregion
}
