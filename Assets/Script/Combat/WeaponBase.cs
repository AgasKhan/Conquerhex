using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/plantilla", fileName = "New weapons")]
public class WeaponBase : FatherWeaponAbility<WeaponBase>, Init
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

    protected override void MyEnable()
    {
        Init();
        base.MyEnable();
    }
    #endregion
}



public class Weapon : Item<WeaponBase>, IGetPercentage
{
    Tim durability;

    public Damage[] damages => itemBase.damages;

    public event System.Action durabilityOff;

    public override void Init()
    {
        durability.Set(itemBase.durability);
    }

    public void Durability()
    {
        Durability(1);
    }

    public void Durability(float damageToDurability)
    {
        if (durability.Substract(damageToDurability) <= 0)
            durabilityOff?.Invoke();
    }

    protected override Pictionarys<string,string> GetDetails()
    {
        var list = base.GetDetails();

        var aux = durability.current + "/" + durability.total;
        
        list.Add("Durability" , aux);

        return list;
    }

    public float Percentage()
    {
        return durability.Percentage();
    }

    public float InversePercentage()
    {
        return durability.InversePercentage();
    }
}
