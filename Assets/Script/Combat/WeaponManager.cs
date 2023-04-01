using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : Manager<WeaponManager>
{
    [SerializeField]
    WeaponBase[] weapons;

    protected override void Awake()
    {
        autoInit = (Init[])weapons;
        base.Awake();
    }
}

public abstract class Manager<T> : MonoBehaviour where T : Manager<T>
{
    protected Init[] autoInit;

    protected virtual void Awake()
    { 

        foreach (var item in autoInit)
        {
            item.Init();
        }

        autoInit = null;
    }
}


public class FatherWeaponHability
{
    #region VARIABLES
    public string name;
    public Sprite image;

    [Header("Estadisticas")]
    public Damage[] damages;
    public float velocity;

}


[System.Serializable]
public class WeaponBase : FatherWeaponHability
{

    public float durability;

    #endregion

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

    
    #endregion
}

/// /////////////////////////////////////////////////
/// 



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


