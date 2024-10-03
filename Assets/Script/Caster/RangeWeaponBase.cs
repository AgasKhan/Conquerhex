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

    public override System.Type GetItemType()
    {
        return typeof(RangeWeapon);
    }
}


public class RangeWeapon : MeleeWeapon
{
    public Tim amination;

    public Tim dispersion;

    public Vector2Int prefabBullet => ((RangeWeaponBase)itemBase).indexPrefabBullet;

    protected override void Init()
    {
        base.Init();

        amination = new Tim(((RangeWeaponBase)itemBase).magazine);
    }

    public override IEnumerable<Entity> ApplyDamage(Ability ability, IEnumerable<Damage> damages, IEnumerable<Entity> damageables)
    {
        CasterEntityComponent owner = ability.caster;

        Vector3 sapawnPos = owner.transform.position + Vector3.up + ability.AimingXZ;

        Vector3 aim = ability.ObjectiveToAim - sapawnPos;

        Entity objective = null;

        if(ability.isPerspective)
        {
            aim = ability.ObjectiveToAim - sapawnPos;

            var angle = Mathf.Clamp(ability.Angle, 0 , 30);

            aim = Quaternion.Euler(Random.Range(angle / -2, angle / 2), Random.Range(angle / -2, angle / 2), 0) * (aim);
        }
        else
        {
            var angle = Mathf.Clamp(ability.Angle, 0, 60);
            aim = ability.AimingXZ;
            aim = Quaternion.Euler(0 , Random.Range(angle / -2, angle / 2), 0) * (aim);

            if (damageables != null)
            {
                foreach (var item in damageables)
                {
                    objective = item;
                    break;
                }
            }

            if (objective != null)
            {
                aim = (objective.transform.position - owner.transform.position).normalized;
            }
        }

        PoolManager.SpawnPoolObject(prefabBullet, out Proyectile proyectile, sapawnPos, Quaternion.identity, null, false);

        proyectile.Throw(owner.container, System.Linq.Enumerable.ToArray(damages), aim);

        return new Entity[] {};
    }


    public override void Durability(float damageToDurability)
    {
        if (amination.total == 0)
            return;

        if (amination.Substract(1) <= 0)
        {
            TriggerOff();
            return;
        }

        base.Durability(damageToDurability);
    }
}
