using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;
public class ModularEquipViewEntityComponent : ComponentOfContainer<Entity>
{
    [System.Serializable]
    public class Data
    {
        public Vector3 offset;        
        public Vector3 forwardRotation;
        public Transform transform;

        public Vector3 Position => transform.position + transform.TransformDirection(offset);

        public Quaternion Rotation => transform.rotation * Quaternion.Euler(forwardRotation);

        public Vector3 Right => Rotation * Vector3.right;
        public Vector3 Forward => Rotation * Vector3.forward;
        public Vector3 Up => Rotation * Vector3.up;
    }

    public WeaponEquip this[WeaponEquip reference]
    {
        get => equipedsWeapon[reference.name].reference;
    }

    [SerializeField]
    Pictionarys<string, Data> whereToEquip = new();

    Dictionary<string, Data> _whereToEquip = new();

    [SerializeField]
    bool showInModel = true;


    /*
    
    hash => el slot y que slotList tengo (si es un combo, kata, arma)

    WeaponEquip => el modelo del arma

    -No deseo modelo de armas repetidos
    -Deseo borrar las armas que ya no utilizo

    */

    Dictionary<string,(int cantidad, WeaponEquip reference)> equipedsWeapon = new();
    Dictionary<int, WeaponEquip> relation = new();


    Data GetPart(string str) => _whereToEquip[str];

    private void OnWeaponEquipInSlot(int arg1, MeleeWeapon arg2)
    {
        var model = arg2?.itemBase.weaponModel;
        WeaponEquip previus;

        if (relation.ContainsKey(arg1))//si ya habia configurado algo en ese slot
        {
            previus = relation[arg1];

            if (model.Equals(previus))
                return;

            //quiero quitar el viejo

            var previusRef = equipedsWeapon[previus.name];

            previusRef.cantidad--; //reduzco su cantidad

            if (previusRef.cantidad <= 0)//si ya no tengo mas, quiero quitarlo
            {
                previus.Destroy();
                equipedsWeapon.Remove(previus.name);
            }
            else
            {
                equipedsWeapon[previus.name] = previusRef;
            }
        }
        else
            relation.Add(arg1, null);

        //agrego arma

        if (model == null)
            return;

        if (equipedsWeapon.ContainsKey(model.name))
        {
            var actual = equipedsWeapon[model.name];
            actual.cantidad++;
            model = actual.reference;
            equipedsWeapon[model.name] = actual;
        }
        else
        {
            var partBody = GetPart("RightAttack");
            model = model.Create(partBody.Position, partBody.Rotation, partBody.transform);            
            equipedsWeapon.Add(model.name, (1, model));
        }

        relation[arg1] = model;
    }

    private void OnPreCast(Ability obj)
    {
        if(obj is WeaponKata kata)
        {
            switch (kata.state)
            {
                case Ability.State.middle:
                    break;

                case Ability.State.start:
                    if(kata.Weapon?.itemBase.weaponModel!=null)
                        this[kata.Weapon.itemBase.weaponModel].Spawn();
                    break;

                case Ability.State.end:
                    if (kata.Weapon?.itemBase.weaponModel != null)
                        this[kata.Weapon.itemBase.weaponModel].Despawn();
                    break;
            }
        }
    }

    public override void OnEnterState(Entity param)
    {
        _whereToEquip = whereToEquip.ToDictionary();
        //whereToEquip.Clear();

        if(param.TryGetInContainer<CasterEntityComponent>(out var caster))
        {
            caster.onEquipInSlotWeapon += OnWeaponEquipInSlot;
            caster.onPreCast += OnPreCast;

            for (int i = 0; i < caster.weapons.Count; i++)
            {
                if (caster.weapons[i].equiped != null)
                    OnWeaponEquipInSlot((caster.weapons, i).GetHashCode(), caster.weapons[i].equiped);
            }

            for (int i = 0; i < caster.katas.Count; i++)
            {
                if(caster.katas[i].equiped!=null)
                    OnWeaponEquipInSlot((caster.katas, i).GetHashCode(), caster.katas[i].equiped.Weapon);
            }

            for (int i = 0; i < caster.combos.Count; i++)
            {
                if(caster.combos[i].equiped != null && caster.combos[i].equiped is WeaponKata kata)
                {
                    OnWeaponEquipInSlot((caster.combos, i).GetHashCode(), kata.Weapon);
                }
            }
        }
    }



    public override void OnExitState(Entity param)
    {
        if (param.TryGetInContainer<CasterEntityComponent>(out var caster))
        {
            caster.onEquipInSlotWeapon -= OnWeaponEquipInSlot;
            caster.onPreCast -= OnPreCast;
        }
    }

    public override void OnStayState(Entity param)
    {
    }

    private void OnDrawGizmosSelected()
    {
        if (!showInModel)
            return;

        foreach (var item in whereToEquip)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(item.value.Position, 0.05f);

            Gizmos.color = Color.blue;
            Utilitys.DrawArrowRay(item.value.Position, item.value.Forward * 0.2f);

            Gizmos.color = Color.green;
            Utilitys.DrawArrowRay(item.value.Position, item.value.Up * 0.2f);

            Gizmos.color = Color.red;
            Utilitys.DrawArrowRay(item.value.Position, item.value.Right * 0.2f);
        }
    }
}