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
    public bool ShowFeedBackArea = true;

    public bool ShowFeedAffectedEntities = true;

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

    /// <summary>
    /// Spawnea una particula en cada afectado
    /// </summary>
    /// <param name="dmg"></param>
    public virtual void InternalParticleSpawnToDamaged(Transform dmg)
    {
        if (indexParticles != null && indexParticles.Length > 0)
            PoolManager.SpawnPoolObject(indexParticles[0], new Vector3(dmg.position.x, dmg.position.y+0.5f, dmg.position.z), Quaternion.identity, dmg);
    }

    /// <summary>
    /// Spawnea una particula en el lugar de ataque
    /// </summary>
    /// <param name="dmg"></param>
    /// <param name="scale"></param>
    public virtual void InternalParticleSpawnToPosition(Transform dmg, Vector3 scale)
    {
        if (indexParticles != null && indexParticles.Length > 1)
        {
            var tr = PoolManager.SpawnPoolObject(indexParticles[1], dmg.position, Quaternion.identity);
            tr.localScale = scale;
        }
            
    }

    public List<Entity> Detect(ref List<Entity> result, Entity caster, Vector3 pos, Vector3 direction, int numObjectives, float minRange, float maxRange, float dot)
        => detection?.Detect(ref result, caster, pos, direction, numObjectives, minRange, maxRange, dot);

    public TriggerController CreateTriggerController()
    {
        return trigger.Create();
    }
}

[System.Serializable]
public abstract class Ability : Item<AbilityBase>, IControllerDir, ICoolDown, IStateWithEnd<CasterEntityComponent>, IAbilityComponent
{
    public event System.Action onCast;

    public List<Entity> affected = new List<Entity>();

    public CasterEntityComponent caster { get; private set;}

    [SerializeReference]
    public Ability original;

    public bool IsCopy => original != null;

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

    public override bool visible => !IsCopy;

    
    public FadeColorAttack FeedBackReference
    {
        get
        {
            if(_feedBackReference==null && itemBase.ShowFeedBackArea)
            {
                /*var aux = */PoolManager.SpawnPoolObject(Vector2Int.up, out _feedBackReference, caster.transform.position, null ,caster.transform);

                //aux.SetParent(caster.transform);
            }

            return _feedBackReference;
        }
        set
        {
            _feedBackReference?.Off();
            _feedBackReference = value;
        }
    }
    

    public Ability CreateCopy(out int index)
    {
        var aux = Create() as Ability;
        aux.original = this;

        onDrop += aux.OnDropOriginal;

        index = aux.Init(container);

        //Debug.Log("Se creo una copia de " + aux.nameDisplay + " en el indice: " + index + "\nCon los casters: " + (aux.caster != null) + " " + (caster != null));

        return aux;
    }

    public void StopCast()
    {
        FeedBackReference = null;

        pressed = MyControllerVOID;

        up = MyControllerVOID;

        End = true;
    }

    public IEnumerable<Entity> ApplyCast(IEnumerable<Entity> entities)
    {
        onCast?.Invoke();

        itemBase.InternalParticleSpawnToPosition(caster.transform, Vector3.one * AttackArea);

        if (entities != null)
            foreach (var dmgEntity in entities)
            {
                itemBase.InternalParticleSpawnToDamaged(dmgEntity.transform);
            }

        return entities;
    }

    public IEnumerable<Entity> Cast()
    {
        return ApplyCast(InternalCast(affected));
    }

    public IEnumerable<Entity> Cast(List<Entity> affected)
    {
        return ApplyCast(InternalCast(affected));
    }

    public override void Destroy()
    {
        if (IsCopy)
            original.onDrop -= OnDropOriginal;
        trigger.Destroy();
        base.Destroy();
    }

    public override void Unequip()
    {
        base.Unequip();
        End = true;
        StopCast();
        if (IsCopy)
            Destroy();
            
    }

    public List<Entity> Detect(Entity caster, Vector3 pos, float timePressed = 0, float? minRange = null, float? maxRange = null, float? dot = null)
    {
        affected = trigger.InternalDetect(caster, pos ,Aiming, timePressed, minRange, maxRange, dot);

        if (affected != null && itemBase.ShowFeedAffectedEntities)
            foreach (var item in affected)
            {
                item.Detect();
            }

        return affected;
    }

    public List<Entity> Detect(Vector3 pos, float timePressed = 0, float? minRange = null, float? maxRange = null, float? dot = null)
    {
        return Detect(caster.container, pos ,timePressed , minRange,maxRange, dot);
    }

    public List<Entity> Detect(float timePressed = 0, float? minRange = null, float? maxRange = null, float? dot = null)
    {
        return Detect(caster.container, caster.transform.position, timePressed, minRange, maxRange, dot);
    }

    protected void SetCooldown()
    {
        if (cooldown == null)
            cooldown = TimersManager.Create(FinalVelocity);
        else
            cooldown.Set(FinalVelocity);
    }

    private void OnDropOriginal()
    {
        original.onDrop -= OnDropOriginal;//original

        Unequip();
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

        if (!container.TryGetInContainer(out CasterEntityComponent caster))
        {
            return;
        }

        this.caster = caster;


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
            End = true;
            return;
        }

        pressed(dir, tim);
    }

    public void ControllerUp(Vector2 dir, float tim)
    {
        caster.abilityControllerMediator -= this;

        if (DontExecuteCast)
        {
            Debug.Log("sali finalizando a estar presionado");
            End = true;
            return;
        }

        up(dir, tim);
    }

    public void OnEnterState(CasterEntityComponent param)
    {
        if ((CostExecution < 0 && !param.NegativeEnergy(-CostExecution)) || (CostExecution > 0 && !param.PositiveEnergy(CostExecution)))
        {
            End = true;
            return;
        }
       //param.abilityControllerMediator += this;
        trigger.OnEnterState(param);
    }

    public void OnStayState(CasterEntityComponent param)
    {
        trigger.OnStayState(param);
    }

    public void OnExitState(CasterEntityComponent param)
    {
        trigger.OnExitState(param);
        StopCast();
    }

    #endregion



    #region internal functions

    protected abstract IEnumerable<Entity> InternalCast(List<Entity> entities);

    
    #endregion

    protected void MyControllerVOID(Vector2 dir, float tim)
    {
    }
}