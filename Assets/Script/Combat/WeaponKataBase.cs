using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public abstract class WeaponKataBase : FatherWeaponAbility<WeaponKataBase>
{
    [Space]

    [Header("tipo de boton")]
    public bool joystick;    

    [Space]

    [Header("FeedBack")]
    public GameObject[] particles;

    [SerializeField]
    public Pictionarys<string, AudioLink> audios = new Pictionarys<string, AudioLink>();

    [Header("Deteccion")]

    [SerializeField]
    public Detect<Entity> detect;

    [Header("Multiplicadores danio")]

    public float velocityCharge = 1;

    public int damageToWeapon = 1;

    public Damage[] damagesMultiply = new Damage[0];

    public Vector2Int[] indexParticles;


    public override Pictionarys<string, string> GetDetails()
    {
        var aux = base.GetDetails();

        if(damagesMultiply.Length>0)
            aux.Add("Modificadores", damagesMultiply.ToString("= x", "\n"));

        if (damages.Length > 0)
            aux.Add("Requisitos", damages.ToString("<","\n"));

        return aux;
    }

    protected override void CreateButtonsAcctions()
    {
        //base.CreateButtonsAcctions();
        buttonsAcctions.Add("Equip", Equip);
    }

    protected override void MyEnable()
    {
        base.MyEnable();

        LoadSystem.AddPostLoadCorutine(()=> {

            indexParticles = new Vector2Int[particles.Length];

            for (int i = 0; i < particles.Length; i++)
            {
                indexParticles[i] = PoolManager.SrchInCategory("Particles", particles[i].name);
            }
        });        
    }

    public Entity[] Attack(Entity caster, MeleeWeapon weapon, params Entity[] entities)
    {
        if (weapon == null)
            return null;

        weapon.Durability(damageToWeapon);

        Damage[] damagesCopy = (Damage[])weapon.itemBase.damages.Clone();

        if (caster is Character)
        {
            var casterCharacter = (Character)caster;

            List<Damage> additives = new List<Damage>(casterCharacter.additiveDamage);

            for (int i = 0; i < damagesCopy.Length; i++)
            {
                for (int ii = additives.Count - 1; ii >= 0; ii--)
                {
                    if(damagesCopy[i].typeInstance == additives[ii].typeInstance)
                    {
                        damagesCopy[i].amount += additives[ii].amount;

                        additives.RemoveAt(ii);

                        continue;
                    }
                }
            }

            additives.AddRange(damagesCopy);

            damagesCopy = additives.ToArray();

        }

        for (int i = 0; i < damagesMultiply.Length; i++)
        {
            for (int ii = 0; ii < damagesCopy.Length; ii++)
            {
                if(damagesMultiply[i].typeInstance == damagesCopy[ii].typeInstance)
                {
                    damagesCopy[ii].amount *= damagesMultiply[i].amount;
                    break;
                }
            }
        }

        Damage(ref damagesCopy, entities);

        return entities;
    }

    protected void Damage(ref Damage[] damages, params Entity[] damageables)
    {
        foreach (var entitys in damageables)
        {
            InternalParticleSpawnToDamaged(entitys.transform);

            foreach (var dmg in damages)
            {
                entitys.TakeDamage(dmg);
            }
        }
    }

    protected virtual void InternalParticleSpawnToDamaged(Transform dmg)
    {
        var aux = PoolManager.SpawnPoolObject(indexParticles[0], dmg.position);

        //aux.SetParent(dmg);

    }

    public abstract Entity[] Detect(Entity caster, Vector2 direction, int numObjectives, float range);

    void Equip(Character chr, Item item)
    {
        chr.actualKata.indexEquipedItem = chr.inventory.IndexOf(item);
    }
}

[System.Serializable]
public abstract class WeaponKata : Item<WeaponKataBase>,Init, IControllerDir
{
    public event System.Action<MeleeWeapon> onEquipedWeapon;
    public event System.Action<MeleeWeapon> onDesEquipedWeapon;
    public event System.Action<MeleeWeapon> onRejectedWeapon;

    public event System.Action<float> updateTimer;

    public event System.Action finishTimer;

    [SerializeReference]
    protected Timer cooldown;
    protected Character caster => equipedWeapon.character;

    protected Entity[] affected;

    FadeColorAttack _reference;

    System.Action<Vector2, float> pressed;

    System.Action<Vector2, float> up;

    public bool cooldownTime => cooldown.Chck;

    public virtual float finalVelocity => itemBase.velocity * weapon.itemBase.velocity;

    public virtual float finalRange => itemBase.range * weapon.itemBase.range;

    public FadeColorAttack reference
    {
        get => _reference;
        set
        {
            _reference?.Off();
            _reference = value;
        }
    }

    public MeleeWeapon weapon
    {
        get => equipedWeapon.equiped;
    }

    EquipedItem<MeleeWeapon> equipedWeapon = new EquipedItem<MeleeWeapon>();

    float actualCharacterVelocity;

    public void ChangeWeapon(MeleeWeapon weapon)
    {
        ChangeWeapon(caster.inventory.IndexOf(weapon));
    }

    public void ChangeWeapon(int weaponIndex)
    {
        if(! (caster.inventory[weaponIndex] is MeleeWeapon) || weaponIndex<0)
        {
            Debug.Log("No es un arma");
            return;
        }

        MeleeWeapon weapon = (MeleeWeapon)caster.inventory[weaponIndex];


        foreach (var ability in itemBase.damages)
        {
            foreach (var dmg in weapon.damages)
            {
                if(ability.typeInstance == dmg.typeInstance && ability.amount>dmg.amount)
                {
                    onRejectedWeapon?.Invoke(weapon);
                    Debug.Log("arma no aceptada");
                    return;
                }
            }
        }

        TakeOutWeapon();

        equipedWeapon.indexEquipedItem = weaponIndex;

        onEquipedWeapon?.Invoke(this.weapon);//Jamas recibira un arma null al menos que le este pasando un null como parametro

        this.weapon.off += Weapon_durabilityOff;

        cooldown.Set(finalVelocity);
    }

    public void TakeOutWeapon()
    {
        this.weapon.off -= Weapon_durabilityOff;

        onDesEquipedWeapon?.Invoke(this.weapon);//puede devolver o no null en base a si ya tenia un arma previa o no
    }

    protected Entity[] Attack()
    {
        return itemBase.Attack(caster, weapon, affected);
    }

    protected Entity[] Detect(Vector2 dir, float timePressed = 0)
    {
        affected = InternalDetect(dir, timePressed);

        foreach (var item in affected)
        {
            item.Detect();
        }

        return affected;
    }

    protected virtual Entity[] InternalDetect(Vector2 dir, float timePressed = 0)
    {
        return itemBase.Detect(caster, dir, itemBase.detect.maxDetects, finalRange);
    }

    void TriggerTimerEvent()
    {
        updateTimer?.Invoke(cooldown.InversePercentage());
    }

    void TriggerTimerFinishEvent()
    {
        finishTimer?.Invoke();
    }

    #region interfaces

    /// <summary>
    /// primer parametro caster, segundo weapon
    /// </summary>
    /// <param name="param"></param>
    public override void Init(params object[] param)
    {
        pressed = MyControllerVOID;
        up = MyControllerVOID;

        if (itemBase == null)
            return;

        if (param.Length > 0)
        {
            equipedWeapon.character = param[0] as Character;

            foreach (var item in itemBase.audios)
            {
                caster.audioManager.AddAudio(item.key, item.value);
            }
        }  

        if (param.Length > 1)
            equipedWeapon.indexEquipedItem = (int)param[1];

        cooldown = TimersManager.Create(itemBase.velocity, TriggerTimerEvent, TriggerTimerFinishEvent);

        if(weapon!=null)
        {
            weapon.Init();
            weapon.off += Weapon_durabilityOff;
            cooldown.Set(finalVelocity);
        }
    }

    private void Weapon_durabilityOff()
    {
        onDesEquipedWeapon?.Invoke(weapon);
        equipedWeapon.indexEquipedItem = -1;
    }

    public void ControllerDown(Vector2 dir, float tim)
    {
        if (caster.enabled == false || caster.gameObject.activeSelf == false)
        {
            reference?.Off();
            return;
        }

        actualCharacterVelocity = caster.move.objectiveVelocity;
        caster.move.objectiveVelocity /= 2;

        InternalControllerDown(dir, tim);
        pressed = InternalControllerPress;
        up = InternalControllerUp;
    }

    public void ControllerPressed(Vector2 dir, float tim)
    {
        if (caster.enabled == false || caster.gameObject.activeSelf == false)
        {
            reference?.Off();
            return;
        }

        pressed(dir, tim);

    }

    public void ControllerUp(Vector2 dir, float tim)
    {
        if (caster.enabled == false || caster.gameObject.activeSelf == false)
        {
            reference?.Off();
            return;
        }

        if (caster is DinamicEntity)
        {
            ((DinamicEntity)caster).move.objectiveVelocity = actualCharacterVelocity;
        }

        up(dir, tim);

        reference?.Off();

        pressed = MyControllerVOID;
        up = MyControllerVOID;
    }

    #endregion

    #region internal functions

    void MyControllerVOID(Vector2 dir, float tim)
    {
    }

    protected abstract void InternalControllerDown(Vector2 dir, float tim);

    protected abstract void InternalControllerPress(Vector2 dir, float tim);

    protected abstract void InternalControllerUp(Vector2 dir, float tim);

    #endregion
}
