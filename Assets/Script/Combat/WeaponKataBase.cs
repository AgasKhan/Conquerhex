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
    public DetectSort<Entity> detect;

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

    public Entity[] Attack(Entity caster, Vector2 direction, MeleeWeapon weapon, int numObjectives ,float range)
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

        return InternalAttack(caster, direction, damagesCopy, numObjectives, range);
    }

    protected void Damage(ref Damage[] damages, Entity caster, params Entity[] damageables)
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

    protected abstract Entity[] InternalAttack(Entity caster, Vector2 direction, Damage[] damages, int numObjectives, float range);
}

[System.Serializable]
public abstract class WeaponKata : Item<WeaponKataBase>,Init, IControllerDir
{
    public event System.Action<MeleeWeapon> equipedWeapon;
    public event System.Action<MeleeWeapon> desEquipedWeapon;
    public event System.Action<MeleeWeapon> rejectedWeapon;

    public event System.Action<float> updateTimer;

    public event System.Action finishTimer;

    [SerializeReference]
    protected Timer cooldown;

    [SerializeReference]
    protected StaticEntity caster;

    FadeColorAttack _reference;

    int weaponIndex = -1;

    System.Action<Vector2, float> pressed;

    System.Action<Vector2, float> up;

    public bool cooldownTime => cooldown.Chck;

    public virtual float finalVelocity => itemBase.velocity * weapon.itemBase.velocity;

    public virtual float finalRange => itemBase.range * weapon.itemBase.range;

    float actualVelocity;

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
        get
        {
            if (weaponIndex >= 0 && weaponIndex < caster.inventory.Count)
                return caster.inventory[weaponIndex] as MeleeWeapon;
            else
                return null;
        }
    }

    public void ChangeWeapon(int weaponIndex)
    {
        if(! (caster.inventory[weaponIndex] is MeleeWeapon))
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
                    rejectedWeapon?.Invoke(weapon);
                    Debug.Log("arma no aceptada");
                    return;
                }
            }
        }

        this.weapon.off -= Weapon_durabilityOff;

        desEquipedWeapon?.Invoke(this.weapon);//puede devolver o no null en base a si ya tenia un arma previa o no

        this.weaponIndex = weaponIndex;

        equipedWeapon?.Invoke(this.weapon);//Jamas recibira un arma null al menos que le este pasando un null como parametro

        this.weapon.off += Weapon_durabilityOff;

        cooldown.Set(finalVelocity);
    }

    protected virtual Entity[] Attack(Vector2 dir, float timePressed = 0)
    {
        return itemBase.Attack(caster, dir, weapon, itemBase.detect.maxDetects, finalRange);
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
            this.caster = param[0] as StaticEntity;

            foreach (var item in itemBase.audios)
            {
                caster.audioManager.AddAudio(item.key, item.value);
            }
        }  

        if (param.Length > 1)
            weaponIndex = (int)param[1];

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
        desEquipedWeapon?.Invoke(weapon);
        weaponIndex = -1;
    }

    public void ControllerDown(Vector2 dir, float tim)
    {
        if (caster.enabled == false || caster.gameObject.activeSelf == false)
        {
            reference?.Off();
            return;
        }

        if (caster is DinamicEntity)
        {
            actualVelocity = ((DinamicEntity)caster).move.objectiveVelocity;
            ((DinamicEntity)caster).move.objectiveVelocity /= 2;
        }

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
            ((DinamicEntity)caster).move.objectiveVelocity = actualVelocity;
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
