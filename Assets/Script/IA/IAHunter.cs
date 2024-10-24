using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAHunter : IAFather, IGetPatrol, Init
{
    [SerializeField]
    public Pictionarys<string, SteeringWithTarget> steerings;
    [SerializeField]
    public Detect<IGetEntity> detectCordero;

    public int energy=15;
    
    public Patrol patrol = new Patrol();

    [SerializeReference]
    protected HunterIntern fsm;

    [SerializeField]
    public float minimalDistance;

    public virtual Transform currentObjective { get => actualWaypoint; }

    public Transform actualWaypoint => patrol.currentWaypoint;


    public AutomaticAttack attk;

    public MoveAbstract move
    {
        get
        {
            return _character.move;
        }
    }

    public Team team
    {
        get
        {
            return _character.team;
        }
    }

    protected virtual void Awake()
    {
        fsm = new HunterIntern(this);
        Init();
    }

    private void OnEnable()
    {
        fsm?.energy.Start();
    }

    private void OnDisable()
    {
        fsm?.energy.Stop();
    }

    public override void OnEnterState(Character param)
    {
        base.OnEnterState(param);
        attk = new AutomaticAttack(param, 0);
    }

    public override void OnStayState(Character param)
    {
        fsm.UpdateState();
        patrol.fsmPatrol.UpdateState();
    }

    public override void OnExitState(Character param)
    {
        attk.StopTimers();
        base.OnExitState(param);
    }

    protected override void Health_death()
    {
        attk.StopTimers();
        base.Health_death();
    }



    public Patrol GetPatrol()
    {
        return patrol;
    }

    public void Init(params object[] param)
    {
        patrol.Init(this);
    }
}

[System.Serializable]
public class HunterIntern : FSM<HunterIntern, IAHunter>
{
    public IState<HunterIntern> patrol = new HunterPatrol();

    public HunterChase chase = new HunterChase();

    public IState<HunterIntern> idle = new HunterIdle();

    public TimedAction energy;

    public event System.Action<Vector3> detectEnemy
    {
        add
        {
            chase.detectEnemy += value;
        }

        remove
        {
            chase.detectEnemy -= value;
        }
    }

    public event System.Action<Vector3> noDetectEnemy
    {
        add
        {
            chase.noDetectEnemy += value;
        }

        remove
        {
            chase.noDetectEnemy -= value;
        }
    }

    void IdleEvent()
    {
        CurrentState = idle;
    }

    public HunterIntern(IAHunter reference) : base(reference)
    {
        energy = TimersManager.Create(reference.energy, IdleEvent);
        Init(idle);
    }
}

public class HunterPatrol : IState<HunterIntern>
{
    IAHunter hunter;
    MoveAbstract move;
    Vector2 dir;

    Vector2 conoDir;

    public void OnEnterState(HunterIntern param)
    {
        hunter = param.context;
        move = hunter.move;
        param.context.patrol.fsmPatrol.OnMove += Move;
        param.context.patrol.fsmPatrol.OnStartWait += StartWait;
    }

    public void OnStayState(HunterIntern param)
    {
        param.context.steerings["corderitos"].targets.Clear();

        //var corderos = param.context.detectCordero.ConeWithRay(param.context.transform, conoDir, (target) => { return param.context.team != target.GetEntity().team && target.GetEntity().team != Team.recursos; });

        var corderos = param.context.detectCordero.AreaWithRay(param.context.transform, (target) => { return target.visible && param.context.team != target.GetEntity().team && target.GetEntity().team != Team.recursos; }).ToEntity();

        if (corderos.Length > 0)
        {
            param.context.steerings["corderitos"].targets.Add(corderos[0]);
            param.CurrentState = param.chase;
            return;
        }

        if (move.vectorVelocity.sqrMagnitude >= 0.01f)
            conoDir = Vector2.Lerp(conoDir, move.vectorVelocity.normalized, Time.deltaTime);

        this.move.ControllerPressed(dir, 0);
    }
    public void OnExitState(HunterIntern param)
    {
        param.context.patrol.fsmPatrol.OnMove -= Move;
        param.context.patrol.fsmPatrol.OnStartWait -= StartWait;
    }

    void StartWait()
    {
        dir = Vector2.zero;
    }

    void Move()
    {
        var move = hunter.steerings["waypoints"].GetMove(hunter.currentObjective);
        dir = hunter.steerings["waypoints"].steering.Calculate(move);
        hunter.patrol.MinimalChck(hunter.minimalDistance);
    }
}

public class HunterChase : IState<HunterIntern>
{
    public event System.Action<Vector3> detectEnemy;

    public event System.Action<Vector3> noDetectEnemy;

    SteeringWithTarget steerings;

    Vector3 enemyPos;

    public void OnEnterState(HunterIntern param)
    {
        param.energy.SetMultiply(1.5f);

        steerings = param.context.steerings["corderitos"];

        enemyPos = steerings.targets[0].transform.position;

        //notify.Start();

        DetectEnemy();
    }

    public void OnStayState(HunterIntern param)
    {
        enemyPos = steerings.targets[0].transform.position;

        var distance = (enemyPos - param.context.transform.position).sqrMagnitude;

        if (distance > param.context.detectCordero.radius * param.context.detectCordero.radius || !steerings.targets[0].visible)
        {
            param.CurrentState = param.patrol;
            return;
        }
        else if (distance >= param.context.detectCordero.radius / 2)
        {
            steerings.SwitchSteering<Pursuit>();
        }
        else if (distance < param.context.detectCordero.radius / 3)
        {
            steerings.SwitchSteering<Seek>();
        }

        if (distance <= (param.context.attk.radius * param.context.attk.radius) && param.context.attk.cooldown)
        {
            param.context.attk.Attack();
        }

        param.context.move.ControllerPressed(steerings[0], 0);
    }

    public void OnExitState(HunterIntern param)
    {
        noDetectEnemy?.Invoke(enemyPos);
        param.energy.SetMultiply(1);
        param.context.steerings["corderitos"].targets.Clear();

        //notify.Stop();

        //param.context.patrol.fsmPatrol.CurrentState = param.context.patrol.fsmPatrol.wait;
    }

    void DetectEnemy()
    {
        detectEnemy?.Invoke(enemyPos);
    }
}


public class HunterIdle : IState<HunterIntern>
{

    public void OnEnterState(HunterIntern param)
    {
        param.energy.Stop();
    }

    public void OnStayState(HunterIntern param)
    {
        param.energy.Substract(-param.energy.deltaTime * 3);
        if (param.energy.current == param.energy.total)
        {
            param.CurrentState = param.patrol;
        }
    }

    public void OnExitState(HunterIntern param)
    {
        param.energy.Start();
    }
}