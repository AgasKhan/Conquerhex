using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(menuName = "Abilities/WeaponKata", fileName = "new WeaponKata")]
public abstract class WeaponKataBase : FatherKataAndAbility<WeaponKataBase>
{
    [Space]

    [Header("tipo de boton")]
    public bool joystick;    

    [Space]

    [Header("FeedBack")]
    public GameObject[] particles;

    [SerializeField]
    public Pictionarys<string, AudioLink> audios = new Pictionarys<string, AudioLink>();

    [Header("Statitics")]

    [Tooltip("cooldown")]
    public float velocity;

    public float velocityCharge = 1;

    public int damageToWeapon = 1;

    public Damage[] damagesMultiply = new Damage[0];

    public Damage[] RequiredDamage = new Damage[0];

    public Vector2Int[] indexParticles;

    [Header("Controller")]
    

    [SerializeField, Header("Deteccion")]
    Detections detection;

    public float range => detection.detect.radius;

    public int maxDetects => detection.detect.maxDetects;

    public float dot => detection.detect.dot;

    public override Pictionarys<string, string> GetDetails()
    {
        var aux = base.GetDetails();

        if(damagesMultiply.Length>0)
            aux.Add("Modificadores", damagesMultiply.ToString(": x", "\n"));

        if (RequiredDamage.Length > 0)
            aux.Add("Requisitos", RequiredDamage.ToString("<","\n"));

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
    }

    public List<Entity> Detect(ref List<Entity> result,Entity caster, Vector2 direction, int numObjectives, float range, float dot) 
        => detection.Detect(ref result, caster, direction, numObjectives, range, dot);

    void Equip(Character chr, int item)
    {
        //chr.attack.actualWeapon. = item;
    }
}

[System.Serializable]
public abstract class WeaponKata : Item<WeaponKataBase>, IControllerDir, ICoolDown, IStateWithEnd<CasterEntityComponent>
{
    public event System.Action<MeleeWeapon> onEquipedWeapon;
    public event System.Action<MeleeWeapon> onDesEquipedWeapon;
    public event System.Action<MeleeWeapon> onRejectedWeapon;
    public event System.Action onAttack;

    [SerializeReference]
    protected Timer cooldown;

    protected CasterEntityComponent caster;

    protected List<Entity> affected = new List<Entity>();

    FadeColorAttack _feedBackReference;

    System.Action<Vector2, float> pressed;

    System.Action<Vector2, float> up;

    [SerializeField]
    protected MeleeWeapon equipedWeapon;

    public DamageContainer multiplyDamage { get; protected set; }

    public bool onCooldownTime => cooldown.Chck;

    public event System.Action<IGetPercentage, float> onCooldownChange
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

    public virtual float FinalVelocity => itemBase.velocity * WeaponEnabled.itemBase.velocity;

    public virtual float FinalRange => itemBase.range * WeaponEnabled.itemBase.range;

    public virtual Vector3 Aiming => caster.aiming;

    public override bool visible => !isCopy;

    public bool isCopy = false;

    public FadeColorAttack FeedBackReference
    {
        get => _feedBackReference;
        set
        {
            _feedBackReference?.Off();
            _feedBackReference = value;
        }
    }

    /// <summary>
    /// Devuelve el arma si esta esta en condiciones de ser utilizada
    /// </summary>
    public MeleeWeapon WeaponEnabled => equipedWeapon.durability.current > 0 && HaveSameContainer(equipedWeapon) ? equipedWeapon : null;

    /// <summary>
    /// devuelve el arma vinculada a la habilidad
    /// </summary>
    public MeleeWeapon Weapon => equipedWeapon;

    public bool End { get; protected set; }

    public WeaponKata CreateCopy()
    {
        var aux = Create() as WeaponKata;
        aux.isCopy = true;
        return aux;
    }

    public virtual void ChangeWeapon(Item weaponParam)
    {
        if(!(weaponParam is MeleeWeapon))
        {
            Debug.Log("No es un arma");
            return;
        }

        MeleeWeapon weapon = (MeleeWeapon)weaponParam;

        foreach (var ability in itemBase.RequiredDamage)
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

        equipedWeapon = weapon;

        WeaponEquiped();
    }

    public void TakeOutWeapon()
    {
        WeaponDesequiped();


        pressed = MyControllerVOID; //Para cancelar el ataque presionado
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

    protected List<Entity> Detect(Vector2 dir, float timePressed = 0, float? range=null, float? dot = null)
    {
        affected = InternalDetect(dir, timePressed, range, dot);

        foreach (var item in affected)
        {
            item.Detect();
        }

        return affected;
    }

    #region interfaces

    protected override void Init()
    {
        pressed = MyControllerVOID;
        up = MyControllerVOID;

        onDrop += WeaponDesequiped;

        multiplyDamage = new DamageContainer(()=> itemBase.damagesMultiply);

        if (itemBase == null)
            return;

        if(!container.TryGetInContainer(out caster))
        {
            return;
        }

        /*
        if (caster.TryGetInContainer<MoveEntityComponent>(out var move))
        {
            actualCharacterVelocity = move.move.objectiveVelocity;
        }
        */

        if(caster.TryGetInContainer<AudioEntityComponent>(out var audio))
            foreach (var item in itemBase.audios)
            {
                audio.AddAudio(item.key, item.value);
            }

        Debug.Log("se creo weapon kata " + caster.name);

        equipedWeapon?.Init(container);

        WeaponEquiped();
    }

    private void WeaponEquiped()
    {
        if (equipedWeapon == null)
            return;

        if(cooldown==null)
            cooldown = TimersManager.Create(FinalVelocity);
        else
            cooldown.Set(FinalVelocity);

        equipedWeapon.off += WeaponDesequiped;
        equipedWeapon.onDrop += WeaponDesequiped;
        onEquipedWeapon?.Invoke(equipedWeapon);
    }

    private void WeaponDesequiped()
    {
        if (equipedWeapon == null)
            return;

        equipedWeapon.off -= WeaponDesequiped;
        equipedWeapon.onDrop -= WeaponDesequiped;
        onDesEquipedWeapon?.Invoke(equipedWeapon);
        equipedWeapon = null;
    }


    public void ControllerDown(Vector2 dir, float tim)
    {
        if (caster == null || !caster.isActiveAndEnabled || WeaponEnabled == null)
        {
            StopAttack();
            End = true;
            return;
        }

        //actualCharacterVelocity = caster.move.objectiveVelocity;
        /*
        if(caster is MoveEntityComponent)
        {
            ((MoveEntityComponent)caster).move.objectiveVelocity += -2;
        }
        */

        InternalControllerDown(dir, tim);
        pressed = InternalControllerPress;
        up = InternalControllerUp;
        
         
    }

    public void ControllerPressed(Vector2 dir, float tim)
    {
        if (caster==null || !caster.isActiveAndEnabled || WeaponEnabled == null)
        {
            StopAttack();
            End = true;
            return;
        }

        pressed(dir, tim);

    }

    public void ControllerUp(Vector2 dir, float tim)
    {
        if (caster == null || !caster.isActiveAndEnabled || WeaponEnabled == null)
        {
            StopAttack();
            End = true;
            return;
        }

        up(dir, tim);

        StopAttack();
    }
    public void StopAttack()
    {
        //if (caster.TryGetInContainer(out MoveEntityComponent move) && actualCharacterVelocity > move.move.objectiveVelocity)
        //    move.move.objectiveVelocity += 2;

        FeedBackReference = null;

        pressed = MyControllerVOID;
        
        up = MyControllerVOID;
    }


    #endregion

    #region internal functions
    protected virtual List<Entity> InternalDetect(Vector2 dir, float timePressed = 0, float? range = null, float? dot = null)
    {
        return itemBase.Detect(ref affected, caster.container, dir, itemBase.maxDetects, range ?? FinalRange, dot ?? itemBase.dot);
    }

    IEnumerable<Entity> InternalAttack(List<Entity> entities)
    {
        if (WeaponEnabled == null)
            return new Entity[0];

        var totalDamage = Damage.Combine(Damage.AdditiveFusion, WeaponEnabled.itemBase.damages, caster.additiveDamage.content);

        totalDamage = Damage.Combine(Damage.MultiplicativeFusion, totalDamage, multiplyDamage.content);

        var aux = WeaponEnabled.Damage(caster.container, totalDamage, entities);

        WeaponEnabled.Durability(itemBase.damageToWeapon);

        return aux;
    }

    void MyControllerVOID(Vector2 dir, float tim)
    {
    }

    protected abstract void InternalControllerDown(Vector2 dir, float tim);

    protected abstract void InternalControllerPress(Vector2 dir, float tim);

    protected abstract void InternalControllerUp(Vector2 dir, float tim);

    public virtual void OnEnterState(CasterEntityComponent param)
    {
        End = false;
        param.attack += this;
        ControllerDown(Aiming,0);
    }

    public virtual void OnStayState(CasterEntityComponent param)
    {
    }

    public virtual void OnExitState(CasterEntityComponent param)
    {
        Debug.Log("sali");
        StopAttack();
        param.attack -= this;
    }

    #endregion
}
