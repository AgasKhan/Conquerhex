using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Melee", fileName = "New weapons")]
public class MeleeWeaponBase : FatherWeaponAbility<MeleeWeaponBase>
{
    [Header("Kata que sera ejecutada cuando se desee atacar con el arma base")]
    public WeaponKataBase defaultKata;

    [Tooltip("cooldown")]
    public float velocity;

    [Tooltip("rango de deteccion")]
    public float range;

    public Damage[] damages = new Damage[1];

    public float durability;

    #region FUNCIONES

    public override Pictionarys<string, string> GetDetails()
    {
        var list = base.GetDetails();

        list.Add("Damages", damages.ToString(": ", "\n"));

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

    protected override System.Type SetItemType()
    {
        return typeof(MeleeWeapon);
    }

    protected override void CreateButtonsAcctions()
    {
        buttonsAcctions.Add("Equip", Equip);
        //base.CreateButtonsAcctions();
    }

    void Equip(Character chr, Item item)
    {
        chr.attack.actualKata.equiped?.ChangeWeapon(item);
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

    public WeaponKata defaultKata;

    public virtual IEnumerable<Entity> Damage(Entity owner, IEnumerable<Damage> damages, IEnumerable<Entity> damageables)
    {
        foreach (var entity in damageables)
        {
            bool auxiliarDamaged = false;

            System.Action<Damage> chckDmg = (dmg) => auxiliarDamaged = true;

            entity.onTakeDamage += chckDmg;

            entity.TakeDamage(damages);

            entity.onTakeDamage -= chckDmg;

            if (auxiliarDamaged)
                yield return entity;
        }
    }

    protected override void Init()
    {
        if (itemBase == null)
            return;

        if (durability == null)
            durability = new Tim(itemBase.durability);

        if (defaultKata == null)
        {
            defaultKata = itemBase.defaultKata.Create() as WeaponKata;

            defaultKata.Init(container);
            defaultKata.ChangeWeapon(this);
        }
            
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

    public float current => durability.current;

    public float total => durability.total;

    protected void TriggerOff()
    {
        off?.Invoke();
    }
}
