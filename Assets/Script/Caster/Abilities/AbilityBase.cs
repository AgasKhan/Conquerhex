using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbilityBase : ItemBase, IAbilityStats
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
            PoolManager.SpawnPoolObject(indexParticles[0], dmg.position + Vector3.up * 0.5f, Quaternion.identity, dmg);
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
            var tr = PoolManager.SpawnPoolObject(indexParticles[1], dmg.position + Vector3.up * 0.5f, Quaternion.identity);
            tr.localScale = scale;
        }
    }

    public List<Entity> Detect(ref List<Entity> result, Entity caster, Vector3 pos, Vector3 direction, int numObjectives, float minRange, float maxRange, float dot)
        => detection?.Detect(ref result, caster, pos, direction, numObjectives, minRange, maxRange, dot);
}

[System.Serializable]
public abstract class Ability : ItemEquipable<AbilityBase>, IControllerDir, ICoolDown, IStateWithEnd<CasterEntityComponent>, IAbilityComponent
{
    public event System.Action onCast;

    public List<Entity> affected = new List<Entity>();

    public CasterEntityComponent caster { get; private set;}

    [SerializeReference]
    public Ability original;

    protected System.Action<Vector2, float> pressed;

    protected System.Action<Vector2, float> up;

    protected TriggerController trigger;

    FadeColorAttack _feedBackReference;

    AbilityModificator abilityModificator = new AbilityModificator();

    public bool IsCopy => original != null;

    public Timer cooldown { get; set; }

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

    public virtual float Angle => abilityModificator.Angle;

    public virtual float Dot => abilityModificator.Dot;

    public virtual float FinalVelocity => abilityModificator.FinalVelocity;

    public virtual float FinalMaxRange => abilityModificator.FinalMaxRange;

    public virtual float FinalMinRange => abilityModificator.FinalMinRange;

    public int FinalMaxDetects => abilityModificator.FinalMaxDetects;

    public int MinDetects => abilityModificator.MinDetects;

    public float Auxiliar => abilityModificator.Auxiliar;

    public virtual Vector3 Aiming
    {
        get => caster.aiming;
        set
        {
            caster.aiming = value;
        }
    }

    public virtual bool DontExecuteCast => caster == null || !caster.gameObject.activeInHierarchy;

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

    public IEnumerable<Entity> ApplyCast(IEnumerable<Entity> entities, bool showParticleInPos = true, bool showParticleDamaged=true)
    {
        onCast?.Invoke();

        if(showParticleInPos)
            itemBase.InternalParticleSpawnToPosition(caster.transform, Vector3.one * FinalMaxRange);

        if (entities != null && showParticleDamaged)
            foreach (var dmgEntity in entities)
            {
                itemBase.InternalParticleSpawnToDamaged(dmgEntity.transform);
            }

        return entities;
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

    public List<Entity> Detect(Entity caster, Vector3 pos)
    {
        affected = itemBase.Detect(ref affected,caster, pos ,Aiming, FinalMaxDetects ,FinalMinRange, FinalMaxRange, Dot);

        if (affected != null && itemBase.ShowFeedAffectedEntities)
            foreach (var item in affected)
            {
                item.Detect();
            }

        return affected;
    }

    public List<Entity> Detect()
    {
        return Detect(caster.container, caster.transform.position);
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

        trigger = itemBase.trigger?.Create();

        trigger?.Init(this);

        abilityModificator.Init(this);


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

        trigger?.Set();
    }

    public void ControllerDown(Vector2 dir, float tim)
    {
        if (DontExecuteCast)
        {
            Debug.Log("sali comenzando a estar presionado");
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
            Debug.Log("sali estando presionado");
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
            Debug.Log("sali finalizando a estar presionado");
            End = true;
            return;
        }
        abilityModificator.ControllerUp(dir, tim);
        up(dir, tim);
    }

    public void OnEnterState(CasterEntityComponent param)
    {
        if (!cooldown.Chck || DontExecuteCast || (CostExecution < 0 && !param.NegativeEnergy(-CostExecution)) || (CostExecution > 0 && !param.PositiveEnergy(CostExecution)))
        {
            End = true;
            return;
        }
        //param.abilityControllerMediator += this;
        abilityModificator.OnEnterState(param);
        trigger.OnEnterState(param);
    }

    public void OnStayState(CasterEntityComponent param)
    {
        if (DontExecuteCast)
        {
            End = true;
            return;
        }
        abilityModificator.OnStayState(param);
        trigger.OnStayState(param);
    }

    public void OnExitState(CasterEntityComponent param)
    {
        if (!DontExecuteCast)
        {
            trigger.OnExitState(param);
        }

        abilityModificator.OnExitState(param);

        if (cooldown.Chck)
            cooldown.Reset();
        StopCast();
    }

    #endregion



    #region internal functions

    protected abstract IEnumerable<Entity> InternalCast(List<Entity> entities, out bool showParticleInPos, out bool showParticleDamaged);

    
    #endregion

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
