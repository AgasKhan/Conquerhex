using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public abstract class WeaponKataBase : FatherWeaponAbility<WeaponKataBase>
{
    [Space]

    [Header("tipo de boton")]
    public bool joystick;

    [Space]

    [Header("Particulas a mostrar")]
    public GameObject[] particles;

    [Header("Deteccion")]

    [SerializeField]
    public Detect<Entity> detect;

    [Header("Multiplicadores danio")]
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

    public void Attack(Entity caster, Vector2 direction, Weapon weapon)
    {       

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

        InternalAttack(caster, direction, damagesCopy);
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

    /*
    public abstract void ControllerDown(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd, Vector2Int[] particles);
    public abstract void ControllerPressed(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd, Vector2Int[] particles);
    public abstract void ControllerUp(Entity caster, Vector2 dir, float button, Weapon weapon, Timer cooldownEnd, Vector2Int[] particles);
    */
    protected virtual void InternalParticleSpawnToDamaged(Transform dmg)
    {
        var aux = PoolManager.SpawnPoolObject(indexParticles[0], dmg.position);

        //aux.SetParent(dmg);

    }


    protected abstract void InternalAttack(Entity caster, Vector2 direction, Damage[] damages);
}

[System.Serializable]
public abstract class WeaponKata : Item<WeaponKataBase>,Init, IControllerDir
{
    public event System.Action<Weapon> equipedWeapon;
    public event System.Action<Weapon> desEquipedWeapon;
    public event System.Action<Weapon> rejectedWeapon;

    public event System.Action<float> updateTimer;

    public event System.Action finishTimer;

    FadeOnOff _reference;

    public FadeOnOff reference
    {
        get => _reference;
        set
        {
            _reference?.Off();
            _reference = value;
        }
    }

    public Weapon weapon
    {
        get => _weapon;
        set => ChangeWeapon(value);
    }

    System.Action<Vector2, float> pressed;

    System.Action<Vector2, float> up;


    [SerializeField]
    Weapon _weapon;
    [SerializeReference]
    protected Timer cooldown;
    [SerializeReference]
    protected Entity caster;
    //protected Vector2Int[] indexParticles;

    void TriggerTimerEvent()
    {
        updateTimer?.Invoke(cooldown.InversePercentage());
    }

    void TriggerTimerFinishEvent()
    {
        finishTimer?.Invoke();
    }

    void ChangeWeapon(Weapon weapon)
    {
        foreach (var ability in itemBase.damages)
        {
            foreach (var dmg in weapon.damages)
            {
                if(ability.typeInstance == dmg.typeInstance && ability.amount>dmg.amount)
                {
                    rejectedWeapon(weapon);
                    Debug.Log("arma no aceptada");
                    return;
                }
            }
        }

        desEquipedWeapon?.Invoke(this._weapon);//puede devolver o no null en base a si ya tenia un arma previa o no

        this._weapon = weapon;

        equipedWeapon?.Invoke(this._weapon);//Jamas recibira un arma null al menos que le este pasando un null como parametro
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
            this.caster = param[0] as Entity;

        if (param.Length > 1)
            _weapon = param[1] as Weapon;

        cooldown = TimersManager.Create(itemBase.velocity, TriggerTimerEvent, TriggerTimerFinishEvent);

        if(weapon!=null)
            weapon.Init();

        
    }

    public void ControllerDown(Vector2 dir, float tim)
    {
        if (caster.enabled == false || caster.gameObject.activeSelf == false)
            return;

        if(cooldown.Chck)
        {
            InternalControllerDown(dir, tim);
            pressed = InternalControllerPress;
            up = InternalControllerUp;
        }
    }

    public void ControllerPressed(Vector2 dir, float tim)
    {
        if (caster.enabled == false || caster.gameObject.activeSelf == false)
            return;

        pressed(dir, tim);
    }

    public void ControllerUp(Vector2 dir, float tim)
    {
        if (caster.enabled == false || caster.gameObject.activeSelf == false)
            return;

        up(dir, tim);

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

