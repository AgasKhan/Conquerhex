using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    static public WeaponManager instance;

    public Weapon[] weapons;
    void Awake()
    {
        instance=this;

        new Slash();

        foreach (var item in weapons)
        {
            item.Init();
        }
    }
}


[System.Serializable]
public class Weapon
{
    #region VARIABLES
    public string name;
    public Sprite image;

    [Header("Estadisticas")]
    public Damage[] damages;
    public Tim durability;
    public float velocity;

    public System.Action durabilityOff;
    #endregion

    #region FUNCIONES

    public void Init()
    {
        foreach (var item in damages)
        {
            item.Init();
        }
    }

    #region VIRTUALES

    public virtual void Durability()
    {
        if (durability.Substract(1) <= 0)
        {
            durabilityOff();
        }
    }

    public virtual Weapon CoptyTo()
    {
        Weapon aux = new Weapon();

        aux.name = name;
        aux.image = image;
        aux.damages = damages;
        aux.durability = new Tim(durability.Reset());
        aux.velocity = velocity;

        return aux;
    }

    #endregion

    #endregion

    #region UNITY FUNCIONES



    #endregion
}

/// /////////////////////////////////////////////////