using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAHunter : IAFather
{
    [SerializeField]
    public Pictionarys<string, SteeringWithTarger> steerings;
    [SerializeField]
    public Detect<IGetEntity> detectCordero;
    
    public Patrol patrol = new Patrol();

    [SerializeReference]
    HunterIntern fsm;

    public AutomaticAttack attk;

    public MoveAbstract move
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

    private void Start()
    {
        fsm = new HunterIntern(this);
        patrol.Init(this);
    }

    public override void OnEnterState(Character param)
    {
        character = param;
        attk = new AutomaticAttack(param.prin);
    }

    public override void OnExitState(Character param)
    {

    }

    public override void OnStayState(Character param)
    {
        fsm.UpdateState();
        patrol.fsmPatrol.UpdateState();


        foreach (var itemInPictionary in steerings)
        {
            for (int i = 0; i < itemInPictionary.value.Count; i++)
            {
                move.ControllerPressed(itemInPictionary.value[i], 0);
            }
        }
    }

}

[System.Serializable]
public class HunterIntern : FSM<HunterIntern, IAHunter>
{
    public IState<HunterIntern> patrol = new HunterPatrol();

    public IState<HunterIntern> chase = new HunterChase();

    public IState<HunterIntern> idle = new HunterIdle();

    public TimedAction energy;

    void IdleEvent()
    {
        CurrentState = idle;
    }

    public HunterIntern(IAHunter reference) : base(reference)
    {
        energy = TimersManager.Create(15, IdleEvent);
        Init(idle);

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
        param.energy.Substract(-param.energy.deltaTime*3);
        if(param.energy.current == param.energy.total)
        {
            param.CurrentState = param.patrol;
        }
    }

    public void OnExitState(HunterIntern param)
    {
        param.energy.Start();
    }

}

public class HunterPatrol : IState<HunterIntern>
{
    IAHunter hunter;
    MoveAbstract move;
    Vector2 dir;
    public void OnEnterState(HunterIntern param)
    {
        hunter = param.context;
        move = hunter.move;
        param.context.patrol.fsmPatrol.OnMove += Move;
    }

    public void OnStayState(HunterIntern param)
    {
        float distance = float.PositiveInfinity;
        var corderos = param.context.detectCordero.Area(param.context.transform.position, (target) => { return param.context.team != target.GetEntity().team && target.GetEntity().team != Team.recursos; });

        IGetEntity lamb = null;
        for (int i = 0; i < corderos.Count; i++)
        {
            if (distance > (corderos[i].GetTransform().position - param.context.transform.position).sqrMagnitude)
            {
                lamb = corderos[i];
                distance = (corderos[i].GetTransform().position - param.context.transform.position).sqrMagnitude;
            }
        }

        param.context.steerings["corderitos"].targets.Clear();
        if (lamb != null)
        {
            param.context.steerings["corderitos"].targets.Add(lamb);
            param.CurrentState = param.chase;
            return;
        }


        this.move.ControllerPressed(dir, 0);
    }
    public void OnExitState(HunterIntern param)
    {
        param.context.patrol.fsmPatrol.OnMove -= Move;
    }

    void Move()
    {
        var move = hunter.steerings["waypoints"].GetMove(hunter.patrol.currentWaypoint);
        dir = hunter.steerings["waypoints"].steering.Calculate(move);
        hunter.patrol.MinimalChck(1);
    }
}

public class HunterChase : IState<HunterIntern>
{
    public void OnEnterState(HunterIntern param)
    {
        param.energy.SetMultiply(1.5f);
    }

    public void OnStayState(HunterIntern param)
    {
        var steerings = param.context.steerings["corderitos"];

        var corderitos = steerings.targets;

        var distance = (corderitos[0].GetTransform().position - param.context.transform.position).sqrMagnitude;

        if (distance > param.context.detectCordero.radius * param.context.detectCordero.radius)
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

        if (distance < param.context.attk.radius && param.context.attk.timerToAttack.Chck)
        {
            param.context.attk.Attack();
        }
    }

    public void OnExitState(HunterIntern param)
    {
        param.energy.SetMultiply(1);
        param.context.steerings["corderitos"].targets.Clear();
    }
}
