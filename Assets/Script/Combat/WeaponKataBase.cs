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

    public void Attack(Entity caster, Vector2 direction, MeleeWeapon weapon, float range)
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

        InternalAttack(caster, direction, damagesCopy, range);
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

    protected abstract void InternalAttack(Entity caster, Vector2 direction, Damage[] damages, float range);
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

    public float finalVelocity => itemBase.velocity * weapon.itemBase.velocity;

    public float finalRange => itemBase.range * weapon.itemBase.range;

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

    protected void Attack(Vector2 dir)
    {
        itemBase.Attack(caster, dir, weapon, finalRange);
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
    public override Pictionarys<string, string> GetDetails()
    {
        var aux = base.GetDetails();

        aux.Add("Attack description", "Genera un area de ataque circular en base a un rango");

        return aux;
    }

    protected override void InternalAttack(Entity caster, Vector2 direction, Damage[] damages, float range)
    {
        var pos = caster.transform.position + direction.Vec2to3(0) * detect.distance;

        var aux = detect.AreaWithRay(caster.transform, (tr) => { return caster != tr; }, range);

        Damage(ref damages, caster, aux);
    }
}


/// <summary>
/// Controlador que ejecuta el ataque cuando se presiona el boton y mientas esta presionado (con mayor espera)
/// </summary>
public class PressWeaponKata : WeaponKata
{
    public Timer pressCooldown;

    public override Pictionarys<string, string> GetDetails()
    {
        var aux = base.GetDetails();

        aux.Add("Attack execution", "Ejecuta el ataque cuando se presiona el boton y mientas esta presionado (con mayor espera)");

        return aux;
    }

    protected override void InternalControllerDown(Vector2 dir, float tim)
    {
        if (!cooldown.Chck)
            return;

        var aux = PoolManager.SpawnPoolObject(Vector2Int.up, out FadeColorAttack reference, caster.transform.position);

        this.reference = reference;
        aux.SetParent(caster.transform);

        aux.localScale *= finalRange*2;

        this.reference.Attack();

        Attack(dir);
        pressCooldown.Reset();
    }

    protected override void InternalControllerPress(Vector2 dir, float tim)
    {
        if (!cooldown.Chck)
        {
            cooldown.Reset();
            return;
        }

        if (pressCooldown.Chck)
        {
            Attack(dir);
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

    public override Pictionarys<string, string> GetDetails()
    {
        var aux = base.GetDetails();

        aux.Add("Attack execution", "Ejecuta el ataque cuando se suelta el boton de la habilidad");

        return aux;
    }

    protected override void InternalControllerDown(Vector2 dir, float button)
    {
        if (!cooldown.Chck)
            return;

        Debug.Log("presionaste ataque: " + itemBase.GetType().Name);

        var aux = PoolManager.SpawnPoolObject(Vector2Int.up, out FadeColorAttack reference, caster.transform.position);

        this.reference = reference;
        aux.SetParent(caster.transform);

        aux.localScale *= finalRange * 2;
    }

    //Durante, al mantener y moverlo
    protected override void InternalControllerPress(Vector2 dir, float button)
    {
        if (!cooldown.Chck)
        {
            cooldown.Reset();
        }

        Debug.Log("estas manteniendo ataque: " + itemBase.GetType().Name);
    }

    //Despues, al sotarlo
    protected override void InternalControllerUp(Vector2 dir, float button)
    {
        if (!cooldown.Chck)
            return;

        Debug.Log("Soltaste ataque: " + itemBase.GetType().Name);

        //comienza a bajar el cooldown

        cooldown.Reset();

        Attack(dir);

        //PoolManager.SpawnPoolObject(itemBase.indexParticles[0], caster.transform.position);
        reference.Off().Attack();
    }
}