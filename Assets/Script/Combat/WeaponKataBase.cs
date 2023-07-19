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
        //buttonsAcctions.Add("Equip", Equip);
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

    public virtual void InternalParticleSpawnToDamaged(Transform dmg)
    {
        if(indexParticles!=null && indexParticles.Length>0)
            PoolManager.SpawnPoolObject(indexParticles[0], dmg.position);

        //aux.SetParent(dmg);

    }

    public abstract Entity[] Detect(Entity caster, Vector2 direction, int numObjectives, float range);

    void Equip(Character chr, int item)
    {
        chr.actualKata.indexEquipedItem = item;
    }
}

[System.Serializable]
public abstract class WeaponKata : Item<WeaponKataBase> ,Init, IControllerDir
{
    public event System.Action<MeleeWeapon> onEquipedWeapon;
    public event System.Action<MeleeWeapon> onDesEquipedWeapon;
    public event System.Action<MeleeWeapon> onRejectedWeapon;
    public event System.Action onAttack;

    [SerializeReference]
    protected Timer cooldown;
    protected Character caster => equipedWeapon.character;

    protected Entity[] affected;

    FadeColorAttack _reference;

    System.Action<Vector2, float> pressed;

    System.Action<Vector2, float> up;

    [SerializeField]
    EquipedItem<MeleeWeapon> equipedWeapon = new EquipedItem<MeleeWeapon>();

    //float actualCharacterVelocity;

    public bool cooldownTime => cooldown.Chck;

    public event System.Action<IGetPercentage> onCooldownChange
    {
        add
        {
            cooldown.onChange += value;
        }
        remove
        {
            cooldown.onChange -= value;
        }
    }

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

    public int indexWeapon => equipedWeapon.indexEquipedItem;



    public void ChangeWeapon(MeleeWeapon weapon)
    {
        ChangeWeapon(caster.inventory.IndexOf(weapon));
    }

    public virtual void ChangeWeapon(int weaponIndex)
    {
        if(! (caster.inventory[weaponIndex] is MeleeWeapon) || weaponIndex < 0)
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



    protected void Attack()
    {
        onAttack?.Invoke();

        var damageds = InternalAttack(affected);

        foreach (var dmgEntity in damageds)
        {
            itemBase.InternalParticleSpawnToDamaged(dmgEntity.transform);
        }
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

    Entity[] InternalAttack(params Entity[] entities)
    {
        if (weapon == null)
            return null;

        weapon.Durability(itemBase.damageToWeapon);

        Damage[] damagesCopy = (Damage[])weapon.itemBase.damages.Clone();

        List<Damage> additives = new List<Damage>(caster.additiveDamage);

        for (int i = 0; i < damagesCopy.Length; i++)
        {
            for (int ii = additives.Count - 1; ii >= 0; ii--)
            {
                if (damagesCopy[i].typeInstance == additives[ii].typeInstance)
                {
                    damagesCopy[i].amount += additives[ii].amount;

                    additives.RemoveAt(ii);

                    continue;
                }
            }
        }

        additives.AddRange(damagesCopy);

        damagesCopy = additives.ToArray();

        for (int i = 0; i < itemBase.damagesMultiply.Length; i++)
        {
            for (int ii = 0; ii < damagesCopy.Length; ii++)
            {
                if (itemBase.damagesMultiply[i].typeInstance == damagesCopy[ii].typeInstance)
                {
                    damagesCopy[ii].amount *= itemBase.damagesMultiply[i].amount;
                    break;
                }
            }
        }

        return weapon.Damage(caster,ref damagesCopy, entities);
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
            equipedWeapon.character = param[0] as Character;

        if (param.Length > 1)
            equipedWeapon.indexEquipedItem = (int)param[1];

        if (equipedWeapon.character!=null)
        {
            foreach (var item in itemBase.audios)
            {
                caster.audioManager.AddAudio(item.key, item.value);
            }
        }  

        cooldown = TimersManager.Create(itemBase.velocity);

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
        if (caster == null || caster.enabled == false || caster.gameObject.activeSelf == false)
        {
            reference?.Off();
            return;
        }

        //actualCharacterVelocity = caster.move.objectiveVelocity;
        caster.move.objectiveVelocity += -2;

        InternalControllerDown(dir, tim);
        pressed = InternalControllerPress;
        up = InternalControllerUp;
    }

    public void ControllerPressed(Vector2 dir, float tim)
    {
        if (caster==null || caster.enabled == false || caster.gameObject.activeSelf == false)
        {
            reference?.Off();
            return;
        }

        pressed(dir, tim);

    }

    public void ControllerUp(Vector2 dir, float tim)
    {
        if (caster == null || caster.enabled == false || caster.gameObject.activeSelf == false)
        {
            reference?.Off();
            return;
        }

        caster.move.objectiveVelocity += 2;

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
