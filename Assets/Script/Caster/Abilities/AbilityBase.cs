using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AbilityModificators;

public abstract class AbilityBase : ItemCrafteable, IAbilityStats
{
    [Space]

    [Header("Tipo de boton")]
    public bool joystick;

    [Space]

    [Header("FeedBack")]
    public bool ShowFeedBackArea = true;

    public bool ShowFeedAffectedEntities = true;

    public AnimationClip animationCastStart;

    public AnimationClip animationCastMiddle;

    public AnimationClip animationCastExit;

    public AnimationInfo animations;

    public PoolGameObjectSpawnProperty inPlaceAffected = new PoolGameObjectSpawnProperty() { index = Vector2Int.one*-1};

    public PoolGameObjectSpawnProperty inPlaceOwner = new PoolGameObjectSpawnProperty() { index = Vector2Int.one * -1 };

    public Pictionarys<string,PoolGameObjectSpawnProperty> auxiliarParticles = new Pictionarys<string, PoolGameObjectSpawnProperty>();

    public AudioLink castAudio = new AudioLink() {volume = 1, pitch =1 };

    [SerializeField]
    public Pictionarys<string, AudioLink> auxiliarAudios = new Pictionarys<string, AudioLink>();

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

    public int weightAction;

    public Damage[] damagesMultiply = new Damage[0];

    [field: SerializeField, Header("Trigger controller")]
    public TriggerControllerBase trigger { get; private set; }

    [SerializeField, Header("Deteccion")]
    Detections detection;

    [SerializeField, Header("modificadores que se aplicaran a la habilidad")]
    public ModificatorBase[] modificators;

    public float FinalMaxRange => detection?.detect?.maxRadius ?? 0;

    public float FinalMinRange => detection?.detect?.minRadius ?? 0;

    public int FinalMaxDetects => detection?.detect?.maxDetects ?? 0;

    public int MinDetects => detection?.detect?.minDetects ?? 0;

    public float Angle => detection?.detect?.angle ?? 360;

    public float Dot => detection?.detect?.dot ?? -1;

    public float FinalVelocity => velocity;

    [field: SerializeField]
    public float Auxiliar { get; set; }

    public override Pictionarys<string, string> GetDetails()
    {
        var aux = base.GetDetails();

        if (damagesMultiply.Length > 0)
            aux.Add("Modificadores".RichText("color", "#f6f1c2"), damagesMultiply.ToString(": x", "\n"));

        aux.Add("Tiempo de espera".RichText("color", "#f6f1c2"), velocity.ToString() + " segundos");

        if(costExecution!=0)
            aux.Add("Costo de energía".RichText("color", "#f6f1c2"), costExecution > 0 ? ("-" + costExecution.ToString()).RichText("color", "#ea925e") : ("+" + costExecution.ToString()).RichText("color", "#5afdf7"));

        aux.Add("Área de efecto".RichText("color", "#f6f1c2"), "Ángulo: ".RichText("color", "#f6f1c2") + detection.detect.angle.ToString() +" grados"  +"\nRango máximo: ".RichText("color", "#f6f1c2") + detection.detect.maxRadius + " unidades");

        return aux;
    }

    public override string GetTooltip()
    {
        var aux = base.GetTooltip();
        aux += "\n\n" + (costExecution == 0? "No consume energía".RichText("color", "#FFFFFF") : costExecution > 0 ? ("Consume " + costExecution.ToString() + " de energía roja").RichText("color", "#ea925e") : ("Consume " + costExecution.ToString() + " de energía azul").RichText("color", "#5afdf7"));

        return aux;
    }

    protected override void CreateButtonsAcctions()
    {
        //base.CreateButtonsAcctions();
        //buttonsAcctions.Add("Equip", Equip);
    }

    /// <summary>
    /// Spawnea un objeto en la posicion marcada por el transform
    /// </summary>
    /// <param name="name"></param>
    /// <param name="tr"></param>
    /// <returns></returns>
    public Transform ParticlesSpawn(string name, Transform tr)
    {
        return auxiliarParticles[name].Spawn(tr.position, Quaternion.identity, tr);
    }


    /// <summary>
    /// Spawnea una particula en cada afectado
    /// </summary>
    /// <param name="dmg"></param>
    public virtual void InternalParticleSpawnToDamaged(Transform dmg)
    {
        if (inPlaceAffected.prefabToSpawn != null)
        {
            inPlaceAffected.Spawn(inPlaceAffected.spawnInLocal ? Vector3.zero : dmg.position, Quaternion.identity, dmg);
        }
    }

    /// <summary>
    /// Spawnea una particula en el lugar de ataque
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="scale"></param>
    public virtual void InternalParticleSpawnToPosition(Transform owner, Vector3 scale)
    {
        if (inPlaceOwner.prefabToSpawn != null)
        {
            inPlaceOwner.Spawn(inPlaceOwner.spawnInLocal ? Vector3.zero : owner.position, Quaternion.identity, owner);
        }
    }

    public List<Entity> Detect(ref List<Entity> result, Entity caster, Vector3 pos, Vector3 direction, int numObjectives, float minRange, float maxRange, float dot)
        => detection?.Detect(ref result, caster, pos, direction, numObjectives, minRange, maxRange, dot);

    protected override void MyEnable()
    {
        base.MyEnable();
        /*
        LoadSystem.AddPostLoadCorutine(() =>
        {
            
            indexParticles = new Vector2Int[particles.Length];

            for (int i = 0; i < particles.Length; i++)
            {
                indexParticles[i] = particles[i].index;
            }
            
        });*/
    }
}

[System.Serializable]
public abstract class Ability : ItemEquipable<AbilityBase>, IControllerDir, ICoolDown, IStateWithEnd<CasterEntityComponent>, IAbilityComponent
{
    /// <summary>
    /// Evento que se ejecutara cuando efectivamente haga el casteo
    /// </summary>
    public event System.Action<Ability> onApplyCast;
    public System.Action onInternalCastEvent;

    /// <summary>
    /// Evento que ejecutara en el momento preciso de la animacion
    /// </summary>
    public System.Action<Ability> onAction;

    public event System.Action<Ability> onEndAction;

    public event System.Action<AnimationInfo.Data> onAnimation;

    public event System.Action<string> onAnimationPlayed;

    //public event System.Action onEnter;

    //public event System.Action onExit;

    public List<Entity> affected = new List<Entity>();

    public CasterEntityComponent caster { get; private set;}

    public DamageContainer additiveDamage { get; private set; }

    [SerializeReference]
    public Ability original;

    protected System.Action<Vector2, float> pressed;

    protected System.Action<Vector2, float> up;

    protected TriggerController trigger;

    protected AudioEntityComponent audio;
    protected AimingEntityComponent aiming;

    protected Dictionary<string, Timer> timers = new();

    FadeColorAttack _feedBackReference;

    AbilityModificator abilityModificator = new AbilityModificator();

    public bool IsCopy => original != null;

    public Timer cooldown { get; set; }

    public DamageContainer multiplyDamage { get; protected set; }

    public bool End { get; set; }

    public int weightAction => itemBase.weightAction;

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

    public virtual float Angle => abilityModificator.Angle;

    public virtual float Dot => abilityModificator.Dot;

    public virtual float FinalVelocity => abilityModificator.FinalVelocity;

    public virtual float FinalMaxRange => abilityModificator.FinalMaxRange;

    public virtual float FinalMinRange => abilityModificator.FinalMinRange;

    public int FinalMaxDetects => abilityModificator.FinalMaxDetects;

    public int MinDetects => abilityModificator.MinDetects;

    public float Auxiliar => abilityModificator.Auxiliar;

    public bool isPerspective => aiming.mode != AimingEntityComponent.Mode.topdown;

    public virtual Vector3 Aiming => aiming.AimingToObjective;

    public virtual Vector3 AimingXZ => aiming.AimingToObjectiveXZ;

    public Vector2 Aiming2D { set => aiming.AimingToObjective2D = value; get => aiming.AimingToObjective2D; }

    public virtual Vector3 ObjectiveToAim
    {
        get => aiming.ObjectivePosition;
        set
        {
            aiming.ObjectivePosition = value;
        }
    }


    public virtual bool DontExecuteCast => caster == null || !caster.gameObject.activeInHierarchy || (!onCooldownTime && caster.HasCooldown);

    public virtual float CostExecution => itemBase.costExecution;

    public virtual float CostHandle => itemBase.costHandle;

    public override bool visible => !IsCopy && !isDefault;

    public FadeColorAttack FeedBackReference
    {
        get
        {
            if(_feedBackReference==null && itemBase.ShowFeedBackArea)
            {
                /*var aux = */PoolManager.SpawnPoolObject(Vector2Int.up, out _feedBackReference, Vector3.zero, null ,caster.transform);
                
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


    /// <summary>
    /// Spawnea una particula en cada afectado
    /// </summary>
    /// <param name="dmg"></param>
    public virtual void InternalParticleSpawnToDamaged(Transform dmg) => itemBase.InternalParticleSpawnToDamaged(dmg);

    /// <summary>
    /// Spawnea una particula en el lugar de ataque
    /// </summary>
    /// <param name="owner"></param>
    /// <param name="scale"></param>
    public virtual void InternalParticleSpawnToPosition(Transform owner, Vector3 scale) => itemBase.InternalParticleSpawnToPosition(owner, scale);

    public Ability CreateCopy(out int index)
    {
        var aux = Create() as Ability;
        aux.original = this;

        onDrop += aux.OnDropOriginal;

        index = aux.Init(container);

        Debug.Log("Se creo una copia de " + aux.nameDisplay + " en el indice: " + index + "\nCon los casters: " + (aux.caster != null) + " " + (caster != null));

        return aux;
    }

    public void StopCast()
    {
        FeedBackReference = null;

        pressed = MyControllerVOID;

        up = MyControllerVOID;

        End = true;
    }

    public virtual void PlayAction(string name)
    {
        PlayEventAction(name);

        if (itemBase.animations.animClips.ContainsKey(name, out int index))
            PlayDataAction(itemBase.animations.animClips[index]);
        else
            Debug.LogWarning("No existe el key de accion: " + name);
    }

    protected void PlayEventAction(string name)
    {
        onAnimationPlayed?.Invoke(name);
    }

    protected void PlayDataAction(AnimationInfo.Data data)
    {
        onAnimation?.Invoke(data);
    }

    private void OnAnimation(AnimationInfo.Data obj)
    {
        obj.SetTimers(timers);
    }

    public virtual void PlaySound(string name)
    {
        if (audio == null)
            return;

        audio.Play($"{itemBase.name}-{name}");
    }

    public IEnumerable<Entity> ApplyCast(IEnumerable<Entity> entities, bool showParticleInPos = true, bool showParticleDamaged=true)
    {
        _feedBackReference?.Attack();

        if (showParticleInPos)
            InternalParticleSpawnToPosition(caster.GetInContainer<AnimatorController>().transformModel, Vector3.one * FinalMaxRange);

        if (entities != null)
            foreach (var dmgEntity in entities)
            {
                if (showParticleDamaged)
                    InternalParticleSpawnToDamaged(dmgEntity.transform);
            }

        PlaySound("Cast");

        onInternalCastEvent?.Invoke();
        onApplyCast?.Invoke(this);

        return entities;
    }

    public IEnumerable<Entity> Cast(System.Action onCastAction)
    {
        onInternalCastEvent = onCastAction;
        return ApplyCast(InternalCast(affected, out bool showParticleInPos, out bool showParticleDamaged), showParticleInPos, showParticleDamaged);
    }

    public IEnumerable<Entity> Cast()
    {
        return ApplyCast(InternalCast(affected, out bool showParticleInPos, out bool showParticleDamaged),showParticleInPos, showParticleDamaged);
    }

    public IEnumerable<Entity> Cast(out bool showParticleInPos, out bool showParticleDamaged)
    {
        return ApplyCast(InternalCast(affected, out showParticleInPos, out showParticleDamaged), showParticleInPos, showParticleDamaged);
    }

    public IEnumerable<Entity> Cast(List<Entity> affected)
    {
        return ApplyCast(InternalCast(affected, out bool showParticleInPos, out bool showParticleDamaged), showParticleInPos, showParticleDamaged);
    }

    public IEnumerable<Entity> Cast(List<Entity> affected, out bool showParticleInPos, out bool showParticleDamaged)
    {
        return ApplyCast(InternalCast(affected, out showParticleInPos, out showParticleDamaged), showParticleInPos, showParticleDamaged);
    }

    public override void Destroy()
    {
        if (IsCopy)
            original.onDrop -= OnDropOriginal;
        abilityModificator.Destroy();
        trigger?.Destroy();
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

    public void FeedbackDetect()
    {
        FeedBackReference?.Area(FinalMaxRange, FinalMinRange).Angle(Angle).Direction(AimingXZ);
    }

    public List<Entity> Detect(Entity caster, Vector3 pos, Vector3 aiming)
    {
        affected = itemBase.Detect(ref affected,caster, pos , aiming, FinalMaxDetects ,FinalMinRange, FinalMaxRange, Dot);

        if (affected != null && itemBase.ShowFeedAffectedEntities)
            foreach (var item in affected)
            {
                item.Detect();
            }

        return affected;
    }

    public List<Entity> Detect()
    {
        return Detect(caster.container, caster.transform.position, Aiming);
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

        timers.Add("Action",TimersManager.Create(1, () => onAction?.Invoke(this)).Stop());
        timers.Add("End", TimersManager.Create(1, () =>
        {
            onEndAction?.Invoke(this);
            onEndAction = null;
        }).Stop());

        trigger = itemBase.trigger?.Create();

        trigger?.Init(this);

        abilityModificator.Init(this);

        SetCooldown();

        trigger?.Set();

        onAnimation += OnAnimation;
    }



    public void ControllerDown(Vector2 dir, float tim)
    {
        if (DontExecuteCast)
        {
            //Debug.Log("sali comenzando a estar presionado");
            End = true;
            return;
        }

        abilityModificator.ControllerDown(dir, tim);
        trigger.ControllerDown(dir, tim);
        pressed = trigger.ControllerPressed;
        up = trigger.ControllerUp;
    }

    public void ControllerPressed(Vector2 dir, float tim)
    {
        if (DontExecuteCast)
        {
            //Debug.Log("sali estando presionado");
            End = true;
            return;
        }
        abilityModificator.ControllerPressed(dir, tim);
        pressed(dir, tim);
    }

    public void ControllerUp(Vector2 dir, float tim)
    {
        caster.abilityControllerMediator -= this;

        if (DontExecuteCast)
        {
            //Debug.Log("sali finalizando a estar presionado");
            End = true;
            return;
        }
        abilityModificator.ControllerUp(dir, tim);
        up(dir, tim);
    }

    public bool PayExecution(float cost)
    {
        return DontExecuteCast || (caster.HasEnergyConsuption && (cost < 0 && !caster.NegativeEnergy(-cost)) || (cost > 0 && !caster.PositiveEnergy(cost)));
    }

    public virtual void OnEnterState(CasterEntityComponent param)
    {   
        this.caster = param;

        additiveDamage = caster.additiveDamage;

        audio = caster.GetInContainer<AudioEntityComponent>();

        aiming = caster.GetInContainer<AimingEntityComponent>();

        if (PayExecution(CostExecution))
        {
            End = true;
            return;
        }

        //param.abilityControllerMediator += this;
        abilityModificator.OnEnterState(param);
        trigger.OnEnterState(param);
        caster.OnEnter(this);

        PlayAction("Start");
        //onEnter?.Invoke();
    }



    public virtual void OnStayState(CasterEntityComponent param)
    {
        if (DontExecuteCast)
        {
            End = true;
            return;
        }
        abilityModificator.OnStayState(param);
        trigger.OnStayState(param);
    }

    public virtual void OnExitState(CasterEntityComponent param)
    {
        onInternalCastEvent = null;

        onAction = null;

        onEndAction = null;

        if (!DontExecuteCast)
        {
            trigger.OnExitState(param);
        }

        abilityModificator.OnExitState(param);

        if (cooldown.Chck)
            cooldown.Reset();

        StopCast();
        caster.OnExit(this);
        //onExit?.Invoke();
    }

    #endregion



    #region internal functions

    protected abstract IEnumerable<Entity> InternalCast(List<Entity> entities, out bool showParticleInPos, out bool showParticleDamaged);


    #endregion

    public override ItemTags GetItemTags()
    {
        return new ItemTags("", "", "Habilidad".RichText("color", "#e2bca5"), "");
    }

    protected void MyControllerVOID(Vector2 dir, float tim)
    {
    }
}

[System.Serializable]
public struct AbilityStats : IAbilityStats
{
    [field: SerializeField]
    public float FinalVelocity { get; set; }

    [field: SerializeField]
    public float FinalMaxRange { get; set; }

    [field: SerializeField]
    public float FinalMinRange { get; set; }

    [field: SerializeField]
    public int FinalMaxDetects { get; set; }

    [field: SerializeField]
    public int MinDetects { get; set; }

    [field: SerializeField]
    public float Angle { get; set; }

    [field: SerializeField]
    public float Dot { get; set; }

    [field: SerializeField]
    public float Auxiliar { get; set; }

    public Damage[] damages;
}
