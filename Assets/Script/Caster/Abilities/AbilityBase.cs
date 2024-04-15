using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityBase : ItemBase
{
    [Space]

    [Header("tipo de boton")]
    public bool joystick;

    [Space]

    [Header("FeedBack")]
    public GameObject[] particles;

    [SerializeField]
    public Pictionarys<string, AudioLink> audios = new Pictionarys<string, AudioLink>();

    [Header("Statitics"),Space()]

    [Tooltip("cooldown")]
    public float velocity;

    [Tooltip("Costo de ejecusion:" +
        "\nSi es positivo consume energia" +
        "\nSi es negativo sumara energia")]
    public float costExecution = 0;

    [Tooltip("Costo por manutencion:" +
        "\nSi es positivo consume energia" +
        "\nSi es negativo sumara energia" +
        "\nEste costo no se quitara de forma automatica")]
    public float costHandle = 0;


    public Damage[] damagesMultiply = new Damage[0];

    public Vector2Int[] indexParticles;

    [SerializeField, Header("Trigger controller")]
    TriggerControllerBase trigger;

    [SerializeField, Header("Deteccion")]
    Detections detection;

    public float maxRange => detection?.detect?.maxRadius ?? 0;

    public float minRange => detection?.detect?.minRadius ?? 0;

    public int maxDetects => detection?.detect?.maxDetects ?? 0;

    public float dot => detection?.detect?.dot ?? -1;

    public override Pictionarys<string, string> GetDetails()
    {
        var aux = base.GetDetails();

        if (damagesMultiply.Length > 0)
            aux.Add("Modificadores", damagesMultiply.ToString(": x", "\n"));

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

        LoadSystem.AddPostLoadCorutine(() => {

            indexParticles = new Vector2Int[particles.Length];

            for (int i = 0; i < particles.Length; i++)
            {
                indexParticles[i] = PoolManager.SrchInCategory("Particles", particles[i].name);
            }
        });
    }

    public virtual void InternalParticleSpawnToDamaged(Transform dmg)
    {
        if (indexParticles != null && indexParticles.Length > 0)
            PoolManager.SpawnPoolObject(indexParticles[0], dmg.position);
    }

    public List<Entity> Detect(ref List<Entity> result, Entity caster, Vector3 direction, int numObjectives, float minRange, float maxRange, float dot)
        => detection?.Detect(ref result, caster, direction, numObjectives, minRange, maxRange, dot);

    public TriggerController CreateTriggerController()
    {
        return trigger.Create();
    }
}


public abstract class Ability : Item<AbilityBase>, IControllerDir, ICoolDown, IStateWithEnd<CasterEntityComponent>, IAbilityComponent
{
    public event System.Action onCast;

    public List<Entity> affected = new List<Entity>();

    public CasterEntityComponent caster;

    public bool isCopy = false;

    public Timer cooldown { get; set; }

    protected TriggerController trigger;

    FadeColorAttack _feedBackReference;

    protected System.Action<Vector2, float> pressed;

    protected System.Action<Vector2, float> up;

    public DamageContainer multiplyDamage { get; protected set; }

    public bool End { get; set; }

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

    public float AttackArea => trigger.FinalMaxRange;

    public virtual float Dot => itemBase.dot;

    public virtual float FinalVelocity => itemBase.velocity;

    public virtual float FinalMaxRange => itemBase.maxRange;

    public virtual float FinalMinRange => itemBase.minRange;

    public virtual Vector3 Aiming
    {
        get => caster.aiming;
        set
        {
            caster.aiming = value;
        }
    }

    public virtual bool DontExecuteCast => caster == null || !caster.gameObject.activeSelf;

    public virtual float CostExecution => itemBase.costExecution;

    public virtual float CostHandle => itemBase.costHandle;

    public override bool visible => !isCopy;

    public FadeColorAttack FeedBackReference
    {
        get => _feedBackReference;
        set
        {
            _feedBackReference?.Off();
            _feedBackReference = value;
        }
    }
    public Ability CreateCopy()
    {
        var aux = Create() as Ability;
        aux.isCopy = true;
        return aux;
    }

    public void StopCast()
    {
        FeedBackReference = null;

        pressed = MyControllerVOID;

        up = MyControllerVOID;
    }


    public void Cast()
    {
        onCast?.Invoke();

        var damageds = InternalCast(affected);

        if(damageds!=null)
            foreach (var dmgEntity in damageds)
            {
                itemBase.InternalParticleSpawnToDamaged(dmgEntity.transform);
            }
    }

    public virtual void Destroy()
    {
        trigger.Destroy();
    }

    public List<Entity> Detect(Vector3 dir, float timePressed = 0, float? minRange = null, float? maxRange = null, float? dot = null)
    {
        affected = trigger.InternalDetect(dir, timePressed, minRange, maxRange, dot);

        if (affected != null)
            foreach (var item in affected)
            {
                item.Detect();
            }

        return affected;
    }

    protected void SetCooldown()
    {
        if (cooldown == null)
            cooldown = TimersManager.Create(FinalVelocity);
        else
            cooldown.Set(FinalVelocity);
    }


    #region interfaces

    protected override void Init()
    {
        pressed = MyControllerVOID;
        up = MyControllerVOID;        

        multiplyDamage = new DamageContainer(() => itemBase.damagesMultiply);

        trigger = itemBase.CreateTriggerController();

        trigger.Init(this);

        if (itemBase == null)
            return;

        if (!container.TryGetInContainer(out caster))
        {
            return;
        }

        if (caster.TryGetInContainer<AudioEntityComponent>(out var audio))
            foreach (var item in itemBase.audios)
            {
                audio.AddAudio(item.key, item.value);
            }

        SetCooldown();

        trigger.Set();
    }

    public void ControllerDown(Vector2 dir, float tim)
    {
        if (DontExecuteCast)
        {
            Debug.Log("sali comenzando a estar presionado");
            StopCast();
            End = true;
            return;
        }

        trigger.ControllerDown(dir, tim);
        pressed = trigger.ControllerPressed;
        up = trigger.ControllerUp;
    }

    public void ControllerPressed(Vector2 dir, float tim)
    {
        if (DontExecuteCast)
        {
            Debug.Log("sali estando presionado");
            StopCast();
            End = true;
            return;
        }

        pressed(dir, tim);
    }

    public void ControllerUp(Vector2 dir, float tim)
    {
        if (DontExecuteCast)
        {
            Debug.Log("sali finalizando a estar presionado");
            StopCast();
            End = true;
            return;
        }

        up(dir, tim);

        StopCast();
    }

    public void OnEnterState(CasterEntityComponent param)
    {
        if ((CostExecution < 0 && -CostExecution > param.NegativeEnergy) || (CostExecution > 0 && CostExecution > param.PositiveEnergy))
        {
            End = true;
            return;
        }

        param.NegativeEnergy += CostExecution;//siempre le quito el costo de la habilidad

        trigger.OnEnterState(param);
    }

    public void OnStayState(CasterEntityComponent param)
    {
        trigger.OnStayState(param);
    }

    public void OnExitState(CasterEntityComponent param)
    {
        trigger.OnExitState(param);
    }

    #endregion



    #region internal functions

    protected abstract IEnumerable<Entity> InternalCast(List<Entity> entities);

    
    #endregion

    protected void MyControllerVOID(Vector2 dir, float tim)
    {
    }
}