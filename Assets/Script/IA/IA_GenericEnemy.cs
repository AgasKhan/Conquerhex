using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PatrolLibrary;

public class IA_GenericEnemy : IAFather, IGetPatrol, Init
{
    [SerializeField]
    public Pictionarys<string, SteeringWithTarget> steerings;
    [SerializeField]
    public Detect<IGetEntity> detection;

    public Patrol patrol = new Patrol();

    [SerializeReference]
    protected GenericEnemyFSM fsm;

    [SerializeField]
    public float minimalDistance;

    public virtual Transform currentObjective { get => actualWaypoint; }

    public Transform actualWaypoint => patrol.currentWaypoint;


    public AutomaticAttack attack;

    public MoveEntityComponent move
    {
        get
        {
            return character.move;
        }
    }

    public Team team
    {
        get
        {
            return character.team;
        }
    }

    protected virtual void Awake()
    {
        fsm = new GenericEnemyFSM(this);
        Init();
    }

    public override void OnEnterState(Character param)
    {
        base.OnEnterState(param);
        attack = new AutomaticAttack(param.caster, 0);
    }

    public override void OnStayState(Character param)
    {
        base.OnStayState(param);
        fsm.UpdateState();
        patrol.fsmPatrol.UpdateState();
    }

    public override void OnExitState(Character param)
    {
        attack.StopTimers();
        attack = null;
        base.OnExitState(param);
    }

    protected override void Health_death()
    {
        attack.StopTimers();
        base.Health_death();
    }

    public Patrol GetPatrol()
    {
        return patrol;
    }

    public void Init()
    {
        patrol.Init(this);
    }
}

[System.Serializable]
public class GenericEnemyFSM : FSM<GenericEnemyFSM, IA_GenericEnemy>
{
    public IState<GenericEnemyFSM> patrol = new GenericPatrol();

    public GenericChase chase = new GenericChase();

    public IState<GenericEnemyFSM> idle = new GenericIdle();

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

    public GenericEnemyFSM(IA_GenericEnemy reference) : base(reference)
    {
        Init(idle);
    }
}

public class GenericPatrol : IState<GenericEnemyFSM>
{
    IA_GenericEnemy enemyIA;
    MoveEntityComponent move;
    Vector3 dir;
    Vector3 conoDir;

    public void OnEnterState(GenericEnemyFSM param)
    {
        enemyIA = param.context;
        move = enemyIA.move;
        param.context.patrol.fsmPatrol.OnMove += Move;
        param.context.patrol.fsmPatrol.OnStartWait += StartWait;
    }

    public void OnStayState(GenericEnemyFSM param)
    {
        param.context.steerings["corderitos"].targets.Clear();

        param.context.detection.AreaWithRay(param.context.transform.position, (target) => { return target.visible && param.context.team.TeamEnemyAttack(target.GetEntity().team); });

        if (param.context.detection.results.Count > 0)
        {
            param.context.steerings["corderitos"].targets.Add(param.context.detection.results[0]);
            param.CurrentState = param.chase;
            return;
        }

        if (move.VectorVelocity.sqrMagnitude >= 0.01f)
            conoDir = Vector3.Lerp(conoDir, move.VectorVelocity.normalized, Time.deltaTime);

        this.move.ControllerPressed(dir, 0);
    }
    public void OnExitState(GenericEnemyFSM param)
    {
        param.context.patrol.fsmPatrol.OnMove -= Move;
        param.context.patrol.fsmPatrol.OnStartWait -= StartWait;
    }

    void StartWait()
    {
        dir = Vector3.zero;
    }

    void Move()
    {
        var move = enemyIA.steerings["waypoints"].GetMove(enemyIA.currentObjective);
        dir = enemyIA.steerings["waypoints"].steering.Calculate(move);
        enemyIA.patrol.MinimalChck(enemyIA.minimalDistance);
    }
}

public class GenericChase : IState<GenericEnemyFSM>
{
    public event System.Action<Vector3> detectEnemy;

    public event System.Action<Vector3> noDetectEnemy;

    SteeringWithTarget steerings;

    Vector3 enemyPos;

    Timer evadeTimer;

    public void OnEnterState(GenericEnemyFSM param)
    {
        steerings = param.context.steerings["corderitos"];

        enemyPos = steerings.targets[0].transform.position;

        DetectEnemy();

        param.context.attack.onAttack += Attack_onAttack;

        evadeTimer = TimersManager.Create(2f, ()=> param.CurrentState = param.idle).Stop().SetInitCurrent(0);
    }

    public void OnStayState(GenericEnemyFSM param)
    {
        enemyPos = steerings.targets[0].transform.position;

        var distance = (enemyPos - param.context.transform.position).sqrMagnitude;

        if (distance > param.context.detection.maxRadius * param.context.detection.maxRadius || !steerings.targets[0].visible)
        {
            param.CurrentState = param.patrol;
            return;
        }
        else if (distance >= param.context.detection.maxRadius / 2 && evadeTimer.Chck)
        {
            steerings.SwitchSteering<Pursuit>();
        }
        else if (distance < param.context.detection.maxRadius / 3 && evadeTimer.Chck)
        {
            steerings.SwitchSteering<Seek>();
        }

        if (distance <= (param.context.attack.radius * param.context.attack.radius) && param.context.attack.cooldown && evadeTimer.Chck)
        {
            param.context.attack.ResetAttack();
        }

        param.context.move.ControllerPressed(steerings[0].Vect3To2XZ(), 0);
    }

    private void Attack_onAttack()
    {
        steerings.SwitchSteering<Evade>();
        evadeTimer.Reset();
    }

    public void OnExitState(GenericEnemyFSM param)
    {
        noDetectEnemy?.Invoke(enemyPos);
        steerings.SwitchSteering<Seek>();
        steerings.SwitchSteering<Pursuit>();
        param.context.steerings["corderitos"].targets.Clear();
        param.context.attack.onAttack -= Attack_onAttack;
    }

    void DetectEnemy()
    {
        detectEnemy?.Invoke(enemyPos);
    }
}


public class GenericIdle : IState<GenericEnemyFSM>
{
    Timer timeToStart = TimersManager.Create(3).Stop();
    public void OnEnterState(GenericEnemyFSM param)
    {
        timeToStart.Reset();
    }

    public void OnStayState(GenericEnemyFSM param)
    {
        if (timeToStart.Chck)
            param.CurrentState = param.patrol;
    }

    public void OnExitState(GenericEnemyFSM param)
    {
        
    }
}
