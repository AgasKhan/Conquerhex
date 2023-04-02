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


[CreateAssetMenu(menuName = "Weapons/plantilla", fileName = "New weapons")]
public class WeaponBase : FatherWeaponAbility, Init
{
    public float durability;

    #region FUNCIONES

    public bool CheckDamage(params Damage[] classDamages)
    {
        List<Damage> damagesList = new List<Damage>(classDamages);
       
        foreach (var dmgWeapon in damages)
        {
            foreach (var dmgTest in damagesList)
            {
                if (dmgTest.type == dmgWeapon.type && dmgTest.amount <= dmgWeapon.amount)
                {
                    damagesList.Remove(dmgWeapon);
                    break;
                }
            }
        }

        if (damagesList.Count <= 0)
            return true;
        else
            return false;
    }

    public void Init()
    {
        foreach (var item in damages)
        {
            item.Init();
        }
    }

    private void Awake()
    {
        WeaponManager.instance.weapons.Add(this);
    }
    #endregion
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


