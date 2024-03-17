using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IASuperBoid : IABoid
{

    public Entity lider = null;

    public Entity[] enemyTargets 
    { 
        get 
        {
            if (steerings["enemy"].targets != null)
                return steerings["enemy"].targets.ToEntity();
            else
                return null;
        } 
    }

    FSMBoid fsmBoid;

    public override void OnEnterState(Character param)
    {
        base.OnEnterState(param);
        fsmBoid = new FSMBoid(this);
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

        steerings["enemy"].targets = detectEnemy.AreaWithRay(_character.transform, (algo) => { return algo.visible && _character.team != algo.GetEntity().team && Team.recursos != algo.GetEntity().team; });


        //Lider
        
        var recursos = detectObjective.AreaWithRay(_character.transform, (target) => 
        {
            var entity = target.GetEntity();

            return entity != null && _character.team == entity.team && (entity is Character) &&  !(((Character)entity).CurrentState is IABoid);
        });

        lider = null;

        for (int i = 0; i < recursos.Count; i++)
        {
            if (distance > (recursos[i].GetEntity().transform.position - _character.transform.position).sqrMagnitude)
            {
                lider = recursos[i].GetEntity();
                distance = (recursos[i].GetEntity().transform.position - _character.transform.position).sqrMagnitude;
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
        boidAttack = new BoidAttack(context.character);

        Init(boidAttack);        
    }
}

public class BoidAttack : IState<FSMBoid>
{
    AutomaticAttack automaticAttack;

    public BoidAttack(Character character)
    {
        automaticAttack = new AutomaticAttack(character.caster, 0);
    }

    //ataque
    public void OnEnterState(FSMBoid param)
    {
        automaticAttack.timerToAttack.Set(5);
    }

    public void OnExitState(FSMBoid param)
    {
        automaticAttack.StopTimers();
    }

    public void OnStayState(FSMBoid param)
    {

        if (param.context.character.health.actualLife < param.context.character.health.maxLife / 2)
            param.CurrentState = param.BoidDamaged;

        if(param.context.lider != null)
        {
            var aux = param.context.transform.position - param.context.lider.transform.position;

            param.context.character.move.move.Acelerator( aux.Vect3To2().normalized, 1f / aux.Vect3To2().magnitude); 

            if (param.context.lider.health.actualLife < param.context.lider.health.maxLife / 2)
            {
                param.CurrentState = param.boidCoward;
            }
        }
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
        if (param.context.lider==null ||  param.context.lider.health.actualLife > param.context.lider.health.maxLife / 2)
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
        if (param.context.character.health.actualLife > param.context.character.health.maxLife / 2)
            param.CurrentState = param.boidAttack;
    }
}