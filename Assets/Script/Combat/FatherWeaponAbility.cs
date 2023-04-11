using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class FatherWeaponAbility<T> : ScriptableObject where T : FatherWeaponAbility<T>
{
    public string nameDisplay;
    public Sprite image;

    [Header("Estadisticas")]
    public Damage[] damages;
    public float velocity;

    protected virtual void OnEnable()
    {
        Manager<T>.pic.Add(nameDisplay, (T)this);
    }

    protected virtual void OnDisable()
    {
        Manager<T>.pic.Remove(nameDisplay);
    }
}


