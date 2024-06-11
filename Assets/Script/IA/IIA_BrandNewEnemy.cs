using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IIA_BrandNewEnemy : IABoid
{
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

    FSMBrandNew fsmBoid;

    public override void OnEnterState(Character param)
    {
        base.OnEnterState(param);
        fsmBoid = new FSMBrandNew(this);
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

            return entity != null && character.team == entity.team && (entity is Character) && !(((Character)entity).CurrentState is IABoid);
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

public class FSMBrandNew : FSM<FSMBrandNew, IIA_BrandNewEnemy>
{
    public BrandNewAttack boidAttack;

    public BrandNewCoward boidCoward = new BrandNewCoward();

    public BrandNewDamaged BoidDamaged = new BrandNewDamaged();

    public FSMBrandNew(IIA_BrandNewEnemy reference) : base(reference)
    {
        boidAttack = new BrandNewAttack(context.character);

        Init(boidAttack);
    }
}

public class BrandNewAttack : IState<FSMBrandNew>
{
    AutomaticAttack automaticAttack;

    public BrandNewAttack(Character character)
    {
        automaticAttack = new AutomaticAttack(character.caster, 0);
    }

    //ataque
    public void OnEnterState(FSMBrandNew param)
    {
        automaticAttack.timerToAttack.Set(5);
    }

    public void OnExitState(FSMBrandNew param)
    {
        automaticAttack.StopTimers();
    }

    public void OnStayState(FSMBrandNew param)
    {
        if (param.context.character.health.actualLife < param.context.character.health.maxLife / 2)
            param.CurrentState = param.BoidDamaged;

        if (param.context.lider != null)
        {
            var aux = param.context.transform.position - param.context.lider.transform.position;

            param.context.character.move.ControllerPressed(aux.normalized.Vect3To2XZ(), 0);

            if (param.context.lider.health.actualLife < param.context.lider.health.maxLife / 2)
            {
                param.CurrentState = param.boidCoward;
            }
        }
    }
}

public class BrandNewCoward : IState<FSMBrandNew>
{
    public void OnEnterState(FSMBrandNew param)
    {
        param.context.steerings["lider"].SwitchSteering<Evade>();
    }

    public void OnExitState(FSMBrandNew param)
    {
        param.context.steerings["lider"].SwitchSteering<Arrive>();
    }

    public void OnStayState(FSMBrandNew param)
    {
        if (param.context.lider == null || param.context.lider.health.actualLife > param.context.lider.health.maxLife / 2)
            param.CurrentState = param.boidAttack;
    }
}

public class BrandNewDamaged : IState<FSMBrandNew>
{
    public void OnEnterState(FSMBrandNew param)
    {
        param.context.steerings["enemy"].SwitchSteering<Evade>();
    }

    public void OnExitState(FSMBrandNew param)
    {
        param.context.steerings["enemy"].SwitchSteering<Arrive>();
    }

    public void OnStayState(FSMBrandNew param)
    {
        if (param.context.character.health.actualLife > param.context.character.health.maxLife / 2)
            param.CurrentState = param.boidAttack;
    }
}

public class BrandNewStalk : IState<FSMBrandNew>
{
    public void OnEnterState(FSMBrandNew param)
    {
        param.context.steerings["enemy"].SwitchSteering<Evade>();
    }

    public void OnExitState(FSMBrandNew param)
    {
        param.context.steerings["enemy"].SwitchSteering<Arrive>();
    }

    public void OnStayState(FSMBrandNew param)
    {
        if (param.context.character.health.actualLife > param.context.character.health.maxLife / 2)
            param.CurrentState = param.boidAttack;
    }
}