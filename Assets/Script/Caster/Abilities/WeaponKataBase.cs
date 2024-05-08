using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "Abilities/WeaponKata", fileName = "new WeaponKata")]
public class WeaponKataBase : AbilityBase
{

    public int damageToWeapon = 1;


    public Damage[] RequiredDamage = new Damage[0];

    public override Pictionarys<string, string> GetDetails()
    {
        var aux = base.GetDetails();

        if (RequiredDamage.Length > 0)
            aux.Add("Requisitos", RequiredDamage.ToString("<", "\n"));

        return aux;
    }

    protected override Type SetItemType()
    {
        return typeof(WeaponKata);
    }
}

[System.Serializable]
public class WeaponKata : Ability
{
    public bool externalKata = true;

    public event System.Action<MeleeWeapon> onEquipedWeapon;
    public event System.Action<MeleeWeapon> onDesEquipedWeapon;
    public event System.Action<MeleeWeapon> onRejectedWeapon;


    [SerializeField]
    protected MeleeWeapon equipedWeapon;

    new public WeaponKataBase itemBase => (WeaponKataBase)base.itemBase;

    public override float FinalVelocity => base.FinalVelocity * (WeaponEnabled?.itemBase.velocity ?? 1);

    public override float FinalMaxRange => base.FinalMaxRange * (WeaponEnabled?.itemBase.range ?? 1);

    public override bool DontExecuteCast => base.DontExecuteCast || WeaponEnabled == null;

    public override bool visible => externalKata && !IsCopy;


    /// <summary>
    /// Devuelve el arma si esta esta en condiciones de ser utilizada
    /// </summary>
    public MeleeWeapon WeaponEnabled => equipedWeapon != null ? (equipedWeapon.durability.current > 0 && HaveSameContainer(equipedWeapon) ? equipedWeapon : null) : null;

    /// <summary>
    /// devuelve el arma vinculada a la habilidad
    /// </summary>
    public MeleeWeapon Weapon => equipedWeapon;

    

    public virtual void ChangeWeapon(Item weaponParam)
    {
        if (!(weaponParam is MeleeWeapon))
        {
            Debug.Log("No es un arma");
            return;
        }

        MeleeWeapon weapon = (MeleeWeapon)weaponParam;

        foreach (var ability in itemBase.RequiredDamage)
        {
            foreach (var dmg in weapon.damages)
            {
                if (ability.typeInstance == dmg.typeInstance && ability.amount > dmg.amount)
                {
                    onRejectedWeapon?.Invoke(weapon);
                    Debug.Log("arma no aceptada");
                    return;
                }
            }
        }

        TakeOutWeapon();

        equipedWeapon = weapon;

        WeaponEquiped();

        trigger.Set();
    }

    public void TakeOutWeapon()
    {
        WeaponDesequiped();


        pressed = MyControllerVOID; //Para cancelar el ataque presionado
    }

    private void WeaponEquiped()
    {
        if (equipedWeapon == null)
            return;

        SetCooldown();

        equipedWeapon.off += WeaponDesequiped;
        equipedWeapon.onDrop += WeaponDesequiped;
        onEquipedWeapon?.Invoke(equipedWeapon);
    }

    private void WeaponDesequiped()
    {
        if (equipedWeapon == null)
            return;

        equipedWeapon.off -= WeaponDesequiped;
        equipedWeapon.onDrop -= WeaponDesequiped;
        onDesEquipedWeapon?.Invoke(equipedWeapon);
        equipedWeapon = null;
    }

    protected override IEnumerable<Entity> InternalCast(List<Entity> entities, out bool showParticleInPos, out bool showParticleDamaged)
    {
        showParticleInPos = true;
        showParticleDamaged = true;

        if (WeaponEnabled == null)
            return new Entity[0];

        var totalDamage = Damage.Combine(Damage.AdditiveFusion, WeaponEnabled.itemBase.damages, caster.additiveDamage.content);

        totalDamage = Damage.Combine(Damage.MultiplicativeFusion, totalDamage, multiplyDamage.content);

        var aux = WeaponEnabled.ApplyDamage(caster.container, totalDamage, entities);

        WeaponEnabled.Durability(itemBase.damageToWeapon);

        End = true;

        return aux;
    }

    protected override void Init()
    {
        base.Init();

        //Debug.Log("se creo weapon kata " + caster.name);

        equipedWeapon?.Init(container);

        WeaponEquiped();

        onDrop += WeaponDesequiped;
    }
}
