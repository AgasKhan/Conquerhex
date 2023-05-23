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
    public Damage[] damagesMultiply = new Damage[0];

    public Vector2Int[] indexParticles;

    public int damageToWeapon=1;


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
        if (weapon == null)
            return;

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

    int weaponIndex=-1;

    public Weapon weapon
    {
        get
        {
            if (weaponIndex >= 0)
                return caster.inventory[weaponIndex] as Weapon;
            else
                return null;
        }
    }

    public bool cooldownTime => cooldown.Chck;

    System.Action<Vector2, float> pressed;

    System.Action<Vector2, float> up;

    [SerializeReference]
    protected Timer cooldown;

    [SerializeReference]
    protected StaticEntity caster;

    void TriggerTimerEvent()
    {
        updateTimer?.Invoke(cooldown.InversePercentage());
    }

    void TriggerTimerFinishEvent()
    {
        finishTimer?.Invoke();
    }

    public void ChangeWeapon(int weaponIndex)
    {
        if(! (caster.inventory[weaponIndex] is Weapon))
        {
            Debug.Log("No es un arma");
            return;
        }

        Weapon weapon = (Weapon)caster.inventory[weaponIndex];


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

        this.weapon.durabilityOff -= Weapon_durabilityOff;

        desEquipedWeapon?.Invoke(this.weapon);//puede devolver o no null en base a si ya tenia un arma previa o no

        this.weaponIndex = weaponIndex;

        equipedWeapon?.Invoke(this.weapon);//Jamas recibira un arma null al menos que le este pasando un null como parametro

        this.weapon.durabilityOff += Weapon_durabilityOff;
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
            weapon.durabilityOff += Weapon_durabilityOff;
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

    protected internal void PlayAudio(string name)
    {
        caster.audioManager.Play(name);
    }

    protected internal void PauseAudio(string name)
    {
        caster.audioManager.Pause(name);
    }

    protected internal void StopAudio(string name)
    {
        caster.audioManager.Stop(name);
    }

    protected abstract void InternalControllerDown(Vector2 dir, float tim);

    protected abstract void InternalControllerPress(Vector2 dir, float tim);

    protected abstract void InternalControllerUp(Vector2 dir, float tim);

    #endregion
}


/// <summary>
/// KataBase que tiene configurado su ataque interno en la deteccion de un area en la posicion del caster
/// </summary>
public abstract class AreaKataBase : WeaponKataBase
{
    protected override void InternalAttack(Entity caster, Vector2 direction, Damage[] damages)
    {
        var aux = detect.Area(caster.transform.position + direction.Vec2to3(0) * detect.distance, (tr) => { return caster != tr; });

        Damage(ref damages, caster, aux.ToArray());
    }
}

public class PressWeaponKata : WeaponKata
{
    public Timer pressCooldown;

    protected override void InternalControllerDown(Vector2 dir, float tim)
    {
        if (!cooldown.Chck)
            return;

        var aux = PoolManager.SpawnPoolObject(Vector2Int.up, out FadeOnOff reference, caster.transform.position);

        this.reference = reference;
        aux.SetParent(caster.transform);

        aux.localScale *= itemBase.detect.diameter;

        this.reference.Attack();

        itemBase.Attack(caster, dir, weapon);
        pressCooldown.Reset();
    }

    protected override void InternalControllerPress(Vector2 dir, float tim)
    {
        if(pressCooldown.Chck)
        {
            itemBase.Attack(caster, dir, weapon);
            reference.Attack();
            pressCooldown.Reset();
        }
    }

    protected override void InternalControllerUp(Vector2 dir, float tim)
    {
        reference.Off();
        cooldown.Reset();
        pressCooldown.Reset();
    }
}


/// <summary>
/// Controlador que ejecuta el ataque cuando se suelta el boton de la habilidad
/// </summary>
public class UpWeaponKata : WeaponKata
{
    protected override void InternalControllerDown(Vector2 dir, float button)
    {
        Debug.Log("presionaste ataque: " + itemBase.GetType().Name);

        var aux = PoolManager.SpawnPoolObject(Vector2Int.up, out FadeOnOff reference, caster.transform.position);

        this.reference = reference;
        aux.SetParent(caster.transform);

        aux.localScale *= itemBase.detect.diameter;
    }

    //Durante, al mantener y moverlo
    protected override void InternalControllerPress(Vector2 dir, float button)
    {
        Debug.Log("estas manteniendo ataque: " + itemBase.GetType().Name);
    }

    //Despues, al sotarlo
    protected override void InternalControllerUp(Vector2 dir, float button)
    {
        Debug.Log("Soltaste ataque: " + itemBase.GetType().Name);

        //comienza a bajar el cooldown

        cooldown.Reset();

        itemBase.Attack(caster, dir, weapon);

        //PoolManager.SpawnPoolObject(itemBase.indexParticles[0], caster.transform.position);
        reference.Off().Attack();
    }
}