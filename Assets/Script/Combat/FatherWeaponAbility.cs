using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class FatherWeaponAbility<T> : ItemBase where T : FatherWeaponAbility<T>
{
    [Space]

    [Header("Estadisticas")]
    public Damage[] damages = new Damage[1];
    public float velocity;

    private void OnEnable()
    {
        MyEnable();
    }

    private void OnDisable()
    {
        MyDisable();
    }

    private void OnDestroy()
    {
        MyDisable();
    }

    protected virtual void MyDisable()
    {
        Manager<T>.pic.Remove(nameDisplay);
    }

    protected virtual void MyEnable()
    {
        Manager<T>.pic.Add(nameDisplay, (T)this);
    }


}


