using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using System.Linq;
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

    public struct Side
    {
        public string nameSide;
        public VisualEffect visualEffect;
    }

    public class Equiped
    {
        public ViewEquipWeapon model;

        public Side actualSide;

        public HashSet<int> references = new HashSet<int>();
    }

    public Equiped this[ViewEquipWeapon reference]
    {
        get
        {
            try
            {
                return equipedsWeapon[reference.name];
            }
            catch
            {
                Debug.LogWarning("Error de:" + reference.name);
                return equipedsWeapon.FirstOrDefault().Value;
            }
        }
    }

    [SerializeField]
    Pictionarys<string, Data> whereToEquip = new();

    Dictionary<string, Data> _whereToEquip = new();

    [SerializeField]
    bool showInModel = true;

    [SerializeField]
    VisualEffect rightVisualEffect;

    [SerializeField]
    VisualEffect leftVisualEffect;
    /*
    
    hash => el slot y que slotList tengo (si es un combo, kata, arma)

    WeaponEquip => el modelo del arma

    -No deseo modelo de armas repetidos
    -Deseo borrar las armas que ya no utilizo

    */
    Dictionary<int, ViewEquipWeapon> relation = new();
    Dictionary<string, Equiped> equipedsWeapon = new();

    System.Action onExitAnimation;

    AnimatorController animController;

    Data GetPart(Side side) => _whereToEquip[side.nameSide];

    Side GetHand(AnimationInfo.HandHandling handHandling)
    {
        var side = new Side();

        if (handHandling == AnimationInfo.HandHandling.Normal)
        {
            side.nameSide = "RightAttack";
            side.visualEffect = rightVisualEffect;
        }
        else
        {
            side.nameSide = "LeftAttack";
            side.visualEffect = leftVisualEffect;
        }

        return side;
    }

    public ViewEquipWeapon SpawnWeapon(AnimationInfo.HandHandling handHandling = AnimationInfo.HandHandling.Normal)
    {
        var aux = (container as Character).caster.actualWeapon;
        if (aux != null)
            return SpawnWeapon(aux.Weapon.itemBase.weaponModel);
        else
            return null;
    }
    public void DeSpawnWeapon()
    {
        var aux = (container as Character).caster.actualWeapon;
        if (aux != null)
            DeSpawnWeapon(aux.Weapon.itemBase.weaponModel);
    }

    public ViewEquipWeapon SpawnWeapon(ViewEquipWeapon _weapon, AnimationInfo.HandHandling handHandling = AnimationInfo.HandHandling.Normal)
    {
        var equiped = this[_weapon];
        var weapon = equiped.model;
        equiped.actualSide = GetHand(handHandling);

        Data partBody = GetPart(equiped.actualSide);
        equiped.model.SetPosition(partBody.Position, partBody.Rotation, partBody.transform);

        if (weapon.Spawn())
        {
            equiped.actualSide.visualEffect.SetTexture("PositionPCache", weapon.positionsPCache);
            equiped.actualSide.visualEffect.SetTexture("NormalPCache", weapon.normalsPCache);
            equiped.actualSide.visualEffect.SetInt("CountPCache", weapon.countPCache);
            equiped.actualSide.visualEffect.SetBool("SpawnDespawn", true);
            equiped.actualSide.visualEffect.Play();
        }
        return weapon;
    }

    public void DeSpawnWeapon(ViewEquipWeapon _weapon)
    {
        var equiped = this[_weapon];

        if (equiped.model.Despawn())
        {
            equiped.actualSide.visualEffect.SetBool("SpawnDespawn", false);
            equiped.actualSide.visualEffect.Play();
        }
    }

    private void OnWeaponEquipInSlot(int arg1, MeleeWeapon arg2)
    {
        var model = arg2?.itemBase.weaponModel;
        ViewEquipWeapon previus;

        if (relation.ContainsKey(arg1))//si ya habia configurado algo en ese slot
        {
            //quiero quitar el viejo
            previus = relation[arg1];

            if (previus != null)
            {
                if (previus.Equals(model))
                    return;

                equipedsWeapon[previus.name].references.Remove(arg1);

                if(equipedsWeapon[previus.name].references.Count==0)
                {
                    equipedsWeapon.Remove(previus.name);
                    previus.Destroy();
                }
            }
        }
        else
            relation.Add(arg1,null);

        //agrego arma

        if (model == null)
            return;

        if(equipedsWeapon.TryGetValue(model.name, out var equiped))
        {
            equiped.references.Add(arg1);
            model = equiped.model;
        }
        else
        {
            var partBody = _whereToEquip["RightAttack"];

            model = model.Create(partBody.Position, partBody.Rotation, partBody.transform);

            equiped = new Equiped();

            equiped.model = model;

            equiped.actualSide = new Side() { nameSide = "RightAttack", visualEffect = rightVisualEffect };

            equiped.references.Add(arg1);

            equipedsWeapon.Add(model.name, equiped);
        }

        relation[arg1] = model;
    }

    private void OnEnterCasting(Ability obj)
    {
        
        if (obj is WeaponKata kata)

            if (kata.Weapon?.itemBase.weaponModel != null)
            {
                AnimationInfo.HandHandling handHandling = kata.itemBase.animations.animClips["Cast"].handHandling;

                if (kata.Weapon.itemBase.animations != null && kata.Weapon.itemBase.animations.animClips.ContainsKey("Cast", out int index))
                {
                    handHandling = kata.Weapon.itemBase.animations.animClips[index].handHandling;
                }
                else
                {
                    handHandling = kata.itemBase.animations.animClips["Cast"].handHandling;
                }

                var equiped = this[kata.Weapon.itemBase.weaponModel];
                var weapon = equiped.model;

                var side = GetHand(handHandling);

                Data partBody = GetPart(side);

                weapon.SetPosition(partBody.Position, partBody.Rotation, partBody.transform);

                if (weapon.Spawn())
                {
                    obj.onAnimationPlayed += weapon.PlayAction;

                    side.visualEffect.SetTexture("PositionPCache", weapon.positionsPCache);
                    side.visualEffect.SetTexture("NormalPCache", weapon.normalsPCache);
                    side.visualEffect.SetInt("CountPCache", weapon.countPCache);
                    side.visualEffect.SetBool("SpawnDespawn", true);
                    side.visualEffect.Play();
                }

                equiped.actualSide = side;
            }
    }


    private void OnExitCasting(Ability obj)
    {
        if (obj is WeaponKata kata)
            if (kata.Weapon?.itemBase.weaponModel != null)
            {
                var equiped = this[kata.Weapon?.itemBase.weaponModel];
                var weapon = equiped.model;

                obj.onAnimationPlayed -= weapon.PlayAction;

                if (animController?.isPlaying ?? false)
                {
                    onExitAnimation = () =>
                    {
                        if (weapon.Despawn())
                        {
                            equiped.actualSide.visualEffect.SetBool("SpawnDespawn", false);
                            equiped.actualSide.visualEffect.Play();
                        }

                        onExitAnimation = null;
                    };
                }
                else
                {
                    if (weapon.Despawn())
                    {
                        equiped.actualSide.visualEffect.SetBool("SpawnDespawn", false);
                        equiped.actualSide.visualEffect.Play();
                    }
                }
            }
    }

    private void OnExitAnim(AnimatorStateInfo obj)
    {
        onExitAnimation?.Invoke();
    }


    public override void OnEnterState(Entity param)
    {
        _whereToEquip = whereToEquip.ToDictionary();
        //whereToEquip.Clear();

        if(param.TryGetInContainer<CasterEntityComponent>(out var caster))
        {
            caster.onEquipInSlotWeapon += OnWeaponEquipInSlot;
            caster.onEnterCasting += OnEnterCasting;
            caster.onExitCasting += OnExitCasting;

            if(param.TryGetInContainer<AnimatorController>(out animController))
            {
                animController.onExitAnim += OnExitAnim;
            
            }

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
            caster.onEnterCasting -= OnEnterCasting;
            caster.onExitCasting -= OnExitCasting;

            if (param.TryGetInContainer<AnimatorController>(out var animController))
            {
                animController.onExitAnim -= OnExitAnim;
            }
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
