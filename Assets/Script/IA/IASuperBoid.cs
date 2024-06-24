using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IASuperBoid : IABoid
{
    public AutomaticCharacterAttack automaticAttack = new AutomaticCharacterAttack();

    public Entity lider = null;

    /*
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
    */

    FSMBoid fsmBoid;

    public override void OnEnterState(Character param)
    {
        base.OnEnterState(param);
        automaticAttack.Init(character, character.caster.katasCombo[0]);
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

        dir = Vector3.zero;

        //enemigo

        steerings["enemy"].targets = detectEnemy.AreaWithRay(character.transform.position, (algo) => { return algo.visible && character.team != algo.GetEntity().team && Team.recursos != algo.GetEntity().team && Team.noTeam != algo.GetEntity().team; });


        //Lider
        
        var recursos = detectObjective.AreaWithRay(character.transform.position, (target) => 
        {
            var entity = target.GetEntity();

            return entity != null && character.team == entity.team && (entity is Character) &&  !(((Character)entity).CurrentState is IABoid);
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
    public BoidAttack boidAttack = new BoidAttack();

    public BoidCoward boidCoward = new BoidCoward();

    public BoidDamaged BoidDamaged = new BoidDamaged();

    public FSMBoid(IASuperBoid reference) : base(reference)
    {
        Init(boidAttack);        
    }
}

public class BoidAttack : IState<FSMBoid>
{
    //ataque
    public void OnEnterState(FSMBoid param)
    {
        //automaticAttack.timerToAttack.Set(5);
    }

    public void OnExitState(FSMBoid param)
    {
        //param.context.automaticAttack.StopTimers();
    }

    public void OnStayState(FSMBoid param)
    {
        if (param.context.character.health.actualLife < param.context.character.health.maxLife / 2)
            param.CurrentState = param.BoidDamaged;

        param.context.automaticAttack.ResetAttack();

        if (param.context.lider != null)
        {
            var aux = param.context.transform.position - param.context.lider.transform.position;

            param.context.character.moveEventMediator.ControllerPressed(aux.normalized.Vect3To2XZ(),0); 

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
        param.context.steerings["lider"].SwitchSteering<Pursuit>();
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
        param.context.steerings["enemy"].SwitchSteering<Pursuit>();
    }

    public void OnStayState(FSMBoid param)
    {
        if (param.context.character.health.actualLife > param.context.character.health.maxLife / 2)
            param.CurrentState = param.boidAttack;
    }
}