using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Melee", fileName = "New weapons")]
public class MeleeWeaponBase : FatherWeaponAbility<MeleeWeaponBase>
{
    public float durability;

    public bool isImproved = false;

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

        return damagesList.Count <= 0;
    }

    protected override void SetCreateItemType()
    {
        _itemType = typeof(MeleeWeapon);
    }

    protected override void CreateButtonsAcctions()
    {
        buttonsAcctions.Add("Equip", Equip);
        //base.CreateButtonsAcctions();
    }

    void Equip(Character chr, int item)
    {
        chr.actualKata.equiped?.ChangeWeapon(item);
    }

    #endregion
}


[System.Serializable]
public class MeleeWeapon : Item<MeleeWeaponBase>, IGetPercentage
{
    [SerializeField]
    public Tim durability;

    public Damage[] damages => itemBase.damages;

    public event System.Action off;

    public virtual Entity[] Damage(Entity owner, ref Damage[] damages, params Entity[] damageables)
    {
        List<Entity> entitiesDamaged = new List<Entity>();

        foreach (var entitys in damageables)
        {
            bool auxiliarDamaged = false;

            System.Action chckDmg = () => auxiliarDamaged = true;

            entitys.onTakeDamage += chckDmg;

            entitys.TakeDamage(damages);

            entitys.onTakeDamage -= chckDmg;

            if(auxiliarDamaged)
                entitiesDamaged.Add(entitys);
        }

        return entitiesDamaged.ToArray();
    }

    public override void Init(params object[] param)
    {
        if (itemBase == null)
            return;

        if(durability==null)
            durability = new Tim(itemBase.durability);
    }

    public virtual void Durability(float damageToDurability)
    {
        if (durability.Substract(damageToDurability) <= 0)
            TriggerOff();
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

    public float actual => durability.actual;

    public float max => durability.max;

    protected void TriggerOff()
    {
        off?.Invoke();
    }
}
