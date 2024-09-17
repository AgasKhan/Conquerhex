using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(menuName = "Weapons/Melee", fileName = "New weapons")]
public class MeleeWeaponBase : ItemCrafteable
{
    [Header("Kata que sera ejecutada cuando se desee atacar con el arma base")]
    public WeaponKataBase defaultKata;

    [Tooltip("cooldown")]
    public float velocity;

    [Tooltip("rango de deteccion")]
    public float range;

    [Tooltip("rango de deteccion minima")]
    public float minimalRange=0;

    public Damage[] damages = new Damage[1];

    public float durability;

    public WeaponEquip weaponModel;

    public AnimationInfo animations;

    public PoolGameObjectSpawnProperty inPlaceOwner=new() { index = Vector2Int.one*-1 };

    public AudioLink castAudio = new AudioLink() { volume = 1, pitch = 1 };

    [SerializeField]
    public Pictionarys<string, AudioLink> auxiliarAudios = new Pictionarys<string, AudioLink>();

    #region FUNCIONES

    public override Pictionarys<string, string> GetDetails()
    {
        var list = base.GetDetails();

        var totalDamage = Damage.Combine(Damage.MultiplicativeFusion, damages, defaultKata.damagesMultiply);

        list.Add("Daño total".RichText("color", "#f6f1c2"), totalDamage.ToArray().ToString(": ", "\n"));

        list.Add("Rango de detección".RichText("color", "#f6f1c2"), range.ToString() + " unidades" );

        list.Add("Cooldown".RichText("color", "#f6f1c2"), defaultKata.velocity.ToString() + " segundos");

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

    public override System.Type GetItemType()
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
        chr.caster.katas.actual.equiped?.ChangeWeapon(item);
    }

    #endregion
}


[System.Serializable]
public class MeleeWeapon : ItemEquipable<MeleeWeaponBase>, IGetPercentage
{
    [SerializeReference]
    public WeaponKata defaultKata;

    [SerializeField]
    public Tim durability;

    public Damage[] damages => itemBase.damages;

    public event System.Action off;

    public virtual IEnumerable<Entity> ApplyDamage(Ability ability, IEnumerable<Damage> damages, IEnumerable<Entity> damageables)
    {
        return Damage.ApplyDamage(ability.caster.container, ability.weightAction, damages, damageables);
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

        list.Add("Durabilidad".RichText("color", "#f6f1c2"), aux);

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

    private void MeleeWeapon_onChangeContainer(InventoryEntityComponent obj)
    {
        defaultKata.ChangeContainer(obj);
        defaultKata.ChangeWeapon(this);
    }

    public override ItemTags GetItemTags()
    {
        return new ItemTags("", "", "Arma".RichText("color", "#add8e6ff"), current.ToString());
    }

    protected override void Init()
    {
        if (itemBase == null)
            return;

        if (durability == null)
            durability = new Tim(itemBase.durability);

        if (defaultKata == null && itemBase.defaultKata != null)
        {
            defaultKata = itemBase.defaultKata.Create() as WeaponKata;

            defaultKata.externalKata = false;
            defaultKata.Init(container);
            defaultKata.ChangeWeapon(this);
            OnAfterChangeContainer += MeleeWeapon_onChangeContainer;
        }
    }
}
