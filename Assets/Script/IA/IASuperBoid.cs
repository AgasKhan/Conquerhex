using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IASuperBoid : IABoid
{
    public Character boid => character;

    public Entity lider = null;

    public Entity[] enemyTargets => steerings["enemigos"].targets.ToEntity();

    FSMBoid fsmBoid;

    public override void OnEnterState(Character param)
    {
        fsmBoid = new FSMBoid(this);
        base.OnEnterState(param);        
    }

    public override void OnStayState(Character param)
    {
        fsmBoid.UpdateState();
        base.OnStayState(param);
    }

    protected override void Detection()
    {
        float distance = float.PositiveInfinity;

        dir = Vector2.zero;

        //enemigo

        steerings["enemy"].targets = detectEnemy.Area(character.transform.position, (algo) => { return character.team != algo.GetEntity().team && Team.recursos != algo.GetEntity().team; });


        //Lider
        
        var recursos = detectObjective.Area(character.transform.position, (target) => 
        {
            var entity = target.GetEntity();

            return entity != null && character.team == entity.team && !((entity as Character).CurrentState is IABoid);
        });

        lider = null;

        for (int i = 0; i < recursos.Count; i++)
        {
            if (distance > (recursos[i].GetEntity().transform.position - character.transform.position).sqrMagnitude)
            {
                lider = recursos[i].GetEntity();
                distance = (recursos[i].GetEntity().transform.position - character.transform.position).sqrMagnitude;
            }
        }

        steerings["lider"].targets.Clear();
        if (lider != null)
            steerings["lider"].targets.Add(lider);
    }

    protected override void Flocking()
    {
        dir += (Separation() * BoidsManager.instance.SeparationWeight +               
              Cohesion() * BoidsManager.instance.CohesionWeight);
    }



}

public class FSMBoid : FSM<FSMBoid, IASuperBoid>
{
    public BoidAttack boidAttack;

    public BoidCoward boidCoward = new BoidCoward();

    public BoidDamaged BoidDamaged = new BoidDamaged();

    public FSMBoid(IASuperBoid reference) : base(reference)
    {
        Init(boidAttack);

        boidAttack = new BoidAttack(context.boid);
    }
}

public class BoidAttack : IState<FSMBoid>
{
    AutomaticAttack automaticAttack;

    public BoidAttack(Character character)
    {
        automaticAttack = new AutomaticAttack(character, 0);
    }

    //ataque
    public void OnEnterState(FSMBoid param)
    {
        
    }

    public void OnExitState(FSMBoid param)
    {
        automaticAttack.StopTimers();
    }

    public void OnStayState(FSMBoid param)
    {
        if (param.context.enemyTargets!=null && (param.context.enemyTargets[0].transform.position - param.context.transform.position).sqrMagnitude < 4)
            automaticAttack.Attack();

        if (param.context.boid.health.actualLife < param.context.boid.health.maxLife / 2)
            param.CurrentState = param.BoidDamaged;

        if (param.context.lider.health.actualLife < param.context.lider.health.maxLife / 2)
            param.CurrentState = param.boidCoward;
    }
}

public class BoidCoward : IState<FSMBoid>
{
    public void OnEnterState(FSMBoid param)
    {
        param.context.steerings["lider"].SwitchSteering<Evade>();
    }

    public void OnExitState(FSMBoid param)
    {
        param.context.steerings["lider"].SwitchSteering<Arrive>();
    }

    public void OnStayState(FSMBoid param)
    {
        if (param.context.lider.health.actualLife > param.context.lider.health.maxLife / 2)
            param.CurrentState = param.boidAttack;
    }
}

public class BoidDamaged : IState<FSMBoid>
{
    public void OnEnterState(FSMBoid param)
    {
        param.context.steerings["enemy"].SwitchSteering<Evade>();
    }

    public void OnExitState(FSMBoid param)
    {
        param.context.steerings["enemy"].SwitchSteering<Arrive>();
    }

    public void OnStayState(FSMBoid param)
    {
        if (param.context.boid.health.actualLife > param.context.boid.health.maxLife / 2)
            param.CurrentState = param.boidAttack;
    }
}