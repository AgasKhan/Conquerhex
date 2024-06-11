using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PatrolLibrary;

public class IA_GenericEnemy : IAFather, IGetPatrol, Init
{
    [SerializeField]
    public Pictionarys<string, SteeringWithTarget> steerings;

    [SerializeField]
    public Detect<IGetEntity> alertDetection;

    [SerializeField]
    public Detect<IGetEntity> attackDetection;

    public Patrol patrol = new Patrol();

    [SerializeReference]
    protected GenericEnemyFSM fsm;

    [SerializeField]
    public float minimalDistance;

    public virtual Transform currentObjective { get => actualWaypoint; }

    public Transform actualWaypoint => patrol.currentWaypoint;

    public Transform alertPoint;

    public AutomaticAttack attack;

    public MoveEntityComponent move
    {
        get
        {
            return character.move;
        }
    }

    public float timeToEvade = 2.5f;
    public float timeToIdle = 0.5f;
    public float alertPointRange = 5f;

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
        //attack = new AutomaticAttack(character.caster, 0);
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
        attack?.StopTimers();
        attack = null;
        base.OnExitState(param);
    }

    protected override void Health_death()
    {
        attack?.StopTimers();
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

    public GenericWaiting waiting = new GenericWaiting();

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
        Init(waiting);
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

        param.context.attackDetection.AreaWithRay(param.context.transform.position, (target) => { return target.visible && param.context.team.TeamEnemyAttack(target.GetEntity().team); });

        if (param.context.attackDetection.results.Count > 0)
        {
            param.context.steerings["corderitos"].targets.Add(param.context.attackDetection.results[0]);
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
        param.context.Detect();

        steerings = param.context.steerings["corderitos"];

        enemyPos = steerings.targets[0].transform.position;

        DetectEnemy();

        param.context.attack.onAttack += Attack_onAttack;

        evadeTimer = TimersManager.Create(param.context.timeToEvade, ()=> { param.CurrentState = param.waiting; /*Debug.Log("Timer Evade Check");*/ }).Stop().SetInitCurrent(0);
    }

    public void OnStayState(GenericEnemyFSM param)
    {
        enemyPos = steerings.targets[0].transform.position;

        var distance = (enemyPos - param.context.transform.position).sqrMagnitude;

        if (evadeTimer.Chck && distance > param.context.attackDetection.maxRadius * param.context.attackDetection.maxRadius || !steerings.targets[0].visible)
        {
            param.CurrentState = param.waiting;
            return;
        }
        else if (evadeTimer.Chck && distance >= param.context.attackDetection.maxRadius / 2)
        {
            steerings.SwitchSteering<Pursuit>();
        }
        else if (evadeTimer.Chck && distance < param.context.attackDetection.maxRadius / 3)
        {
            steerings.SwitchSteering<Seek>();
        }

        if (evadeTimer.Chck && distance <= (param.context.attack.radius * param.context.attack.radius) && param.context.attack.cooldown)
        {
            param.context.attack.ResetAttack();
        }

        param.context.move.ControllerPressed(steerings[0].Vect3To2XZ(), 0);
    }

    private void Attack_onAttack(Ability ability)
    {
        if(!evadeTimer.Chck)
            return;

        steerings.SwitchSteering<Seek>();
        steerings.SwitchSteering<Evade>();
        evadeTimer.Reset();
        //Debug.Log("Timer Evade Starts");
    }

    public void OnExitState(GenericEnemyFSM param)
    {
        noDetectEnemy?.Invoke(enemyPos);
        steerings.SwitchSteering<Seek>();
        steerings.SwitchSteering<Pursuit>();
        param.context.steerings["corderitos"].targets.Clear();

        //Debug.Log("FSM: " + (param==null) + "\nCharacter: " + (param.context == null) + "\nAttack: " + (param.context.attack==null));
        if(param.context.attack != null)
            param.context.attack.onAttack -= Attack_onAttack;
    }

    void DetectEnemy()
    {
        detectEnemy?.Invoke(enemyPos);
    }
}

public class GenericWaiting : IState<GenericEnemyFSM>
{
    Timer timeToStart;
    public void OnEnterState(GenericEnemyFSM param)
    {
        timeToStart = TimersManager.Create(param.context.timeToIdle);
        //timeToStart.Reset();
    }

    public void OnStayState(GenericEnemyFSM param)
    {
        if (!timeToStart.Chck)
            return;

        param.context.steerings["corderitos"].targets.Clear();

        param.context.attackDetection.AreaWithRay(param.context.transform.position, (target) => { return target.visible && param.context.team.TeamEnemyAttack(target.GetEntity().team); });

        if (param.context.attackDetection.results.Count > 0)
        {
            param.context.steerings["corderitos"].targets.Add(param.context.attackDetection.results[0]);
            param.CurrentState = param.chase;
        }
    }

    public void OnExitState(GenericEnemyFSM param)
    {

    }
}

public class GenericAlert : IState<GenericEnemyFSM>
{
    Timer timeToGeneratePoint;
    Vector3 originalPos;
    float alertPointRange;

    SteeringWithTarget steerings;

    public void OnEnterState(GenericEnemyFSM param)
    {
        timeToGeneratePoint = TimersManager.Create(2f, ()=> GeneratePoint()).SetLoop(true);
        originalPos = param.context.transform.position;
        alertPointRange = param.context.alertPointRange;

        steerings = param.context.steerings["alert"];
        param.context.alertPoint.position = GeneratePoint();
        steerings.GetMove(param.context.alertPoint);
    }

    public void OnStayState(GenericEnemyFSM param)
    {
        param.context.steerings["corderitos"].targets.Clear();

        param.context.attackDetection.AreaWithRay(param.context.transform.position, (target) => { return target.visible && param.context.team.TeamEnemyAttack(target.GetEntity().team); });

        if (param.context.attackDetection.results.Count > 0)
        {
            param.context.steerings["corderitos"].targets.Add(param.context.attackDetection.results[0]);
            param.CurrentState = param.chase;
        }
    }

    public void OnExitState(GenericEnemyFSM param)
    {
        timeToGeneratePoint.Stop();
    }

    Vector3 GeneratePoint()
    {
        return new Vector3(Random.Range(originalPos.x - alertPointRange, originalPos.x + alertPointRange), 0, 
                           Random.Range(originalPos.z - alertPointRange, originalPos.z + alertPointRange));
    }
}

public class GenericStalk : IState<GenericEnemyFSM>
{
    Timer timeToGeneratePoint;
    Vector3 originalPos;
    float alertPointRange;

    public void OnEnterState(GenericEnemyFSM param)
    {
        //timeToGeneratePoint = TimersManager.Create(2f, () => GeneratePoint()).SetLoop(true);
        originalPos = param.context.transform.position;
        alertPointRange = param.context.alertPointRange;
    }

    public void OnStayState(GenericEnemyFSM param)
    {
        param.context.steerings["corderitos"].targets.Clear();

        param.context.attackDetection.AreaWithRay(param.context.transform.position, (target) => { return target.visible && param.context.team.TeamEnemyAttack(target.GetEntity().team); });

        if (param.context.attackDetection.results.Count > 0)
        {
            param.context.steerings["corderitos"].targets.Add(param.context.attackDetection.results[0]);
            param.CurrentState = param.chase;
        }
    }

    public void OnExitState(GenericEnemyFSM param)
    {
        timeToGeneratePoint.Stop();
    }
    /*
    Vector3 GeneratePoint()
    {
        var Hector =  new Vector3(Random.Range(originalPos.x - alertPointRange, originalPos.x + alertPointRange), 0,
                           Random.Range(originalPos.z - alertPointRange, originalPos.z + alertPointRange));
        float myX = Mathf.Clamp() Random.Range(originalPos.x - alertPointRange, originalPos.x + alertPointRange)

    }*/
}