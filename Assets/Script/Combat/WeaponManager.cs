using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Managers/weapons", fileName = "weapons")]
public class WeaponManager : Manager<WeaponManager>
{
    [SerializeReference]
    public List<WeaponBase> weapons= new List<WeaponBase>();

    protected void OnEnable()
    {
        InitAll(weapons);
    }
}

/// /////////////////////////////////////////////////
/// 

public class FatherWeaponAbility : ScriptableObject
{
    public string nameDisplay;
    public Sprite image;

    [Header("Estadisticas")]
    public Damage[] damages;
    public float velocity;
}

public class Weapon : Init
{
    public WeaponBase weaponBase;
    
    Tim durability;

    public event System.Action durabilityOff;

    public void Init()
    {
        durability.Set(weaponBase.durability);
    }

    public void Durability()
    {
        if (durability.Substract(1) <= 0)
            durabilityOff?.Invoke();
    }
}


