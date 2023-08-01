using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/Range", fileName = "New weapons")]
public class RangeWeaponBase : MeleeWeaponBase
{
    [Header("Ranged")]
    public int magazine;

    public Entity prefabBullet;

    public Vector2Int indexPrefabBullet;

    protected override void MyEnable()
    {
        base.MyEnable();

        LoadSystem.AddPostLoadCorutine(() => {

            indexPrefabBullet = PoolManager.SrchInCategory("Bullets", prefabBullet.name);
        });
    }

    protected override void SetCreateItemType()
    {
        _itemType = typeof(RangeWeapon);
    }
}


public class RangeWeapon : MeleeWeapon
{
    public Tim amunation;

    public Vector2Int prefabBullet => ((RangeWeaponBase)itemBase).indexPrefabBullet;

    public override void Init(params object[] param)
    {
        base.Init(param);

        amunation = new Tim(((RangeWeaponBase)itemBase).magazine);
    }


    public override Entity[] Damage(Entity owner ,ref Damage[] damages, params Entity[] damageables)
    {
        if (damageables == null || damageables.Length == 0)
            return new Entity[] {};

        PoolManager.SpawnPoolObject(prefabBullet, out Proyectile proyectile , owner.transform.position, Quaternion.identity, owner.transform.parent);

        proyectile.Throw(owner, damages, damageables[0].transform.position - proyectile.transform.position);

        return new Entity[] {};
    }


    public override void Durability(float damageToDurability)
    {
        if (amunation.total == 0)
            return;

        if (amunation.Substract(1) <= 0)
        {
            TriggerOff();
            return;
        }

        base.Durability(damageToDurability);
    }
}
