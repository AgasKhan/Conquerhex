using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/plantilla", fileName = "New weapons")]
public class WeaponBase : FatherWeaponAbility<WeaponBase>//, Init
{
    public float range;

    public float durability;

    #region FUNCIONES

    public override Pictionarys<string, string> GetDetails()
    {
        var list = base.GetDetails();

        list.Add("Damages", damages.ToString("=", "\n"));

        return list;
    }

    public bool CheckDamage(params Damage[] classDamages)
    {
        List<Damage> damagesList = new List<Damage>(classDamages);

        foreach (var dmgWeapon in damages)
        {
            foreach (var dmgTest in damagesList)
            {
                if (dmgTest.typeInstance == dmgWeapon.typeInstance && dmgTest.amount <= dmgWeapon.amount)
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

    /*
    public void Init(params object[] param)
    {
        for (int i = 0; i < damages.Length; i++)
        {
            damages[i].Init();
        }
    }
    */

    protected override void MyEnable()
    {
        base.MyEnable();
        //Init();
    }

    protected override void SetCreateItemType()
    {
        _itemType = typeof(Weapon);
    }
    #endregion
}


[System.Serializable]
public class Weapon : Item<WeaponBase>, IGetPercentage
{
    [SerializeField]
    Tim durability;

    public Damage[] damages => itemBase.damages;

    public event System.Action durabilityOff;

    public override void Init(params object[] param)
    {
        if (itemBase == null)
            return;

        if(durability==null)
            durability = new Tim(itemBase.durability);
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

    public override Pictionarys<string,string> GetDetails()
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
