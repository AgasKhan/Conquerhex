using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Trigger/DashToEntityUpTriggerControllerBase")]
public class DashToEntityUpTrggrCtrllrBase : TriggerControllerBase
{
    [Tooltip("Impulso de velocidad que se dara cuando de el dash en direccion del primer enemigo en el area")]
    public float velocityInDash=10;

    public float timerDash = 1;

    public int dashCount = 1;

    public float cooldownWaitAttack = 1;
    protected override System.Type SetItemType()
    {
        return typeof(DashToEntityUpTrggrCtrllr);
    }
}

[System.Serializable]
public class DashToEntityUpTrggrCtrllr : UpTrggrCtrllr
{
    MoveEntityComponent moveEntity;
    Timer timerToEnd;
    bool buttonPress;
    int dashCount;
    List<Entity> objectivesAttacked = new List<Entity>();

    new public DashToEntityUpTrggrCtrllrBase triggerBase => (DashToEntityUpTrggrCtrllrBase)base.triggerBase;

    public override void Init(Ability ability)
    {
        base.Init(ability);
        timerToEnd = TimersManager.Create(triggerBase.timerDash, () => End = true).Stop();
    }

    public override void Destroy()
    {
        base.Destroy(); 
        timerToEnd.Stop();
    }

    public override void ControllerDown(Vector2 dir, float button)
    {
        base.ControllerDown(dir, button);

        buttonPress = true;
        
        FeedBackReference?.Angle(Angle);
        objectivesAttacked.Clear();
        dashCount = triggerBase.dashCount;
    }

    public override void ControllerUp(Vector2 dir, float button)
    {
        if (affected != null && affected.Count != 0 && caster.TryGetInContainer<MoveEntityComponent>(out moveEntity))
        {
            moveEntity.Velocity((affected[0].transform.position - caster.transform.position).normalized , triggerBase.velocityInDash);
            
            dashCount--;
            objectivesAttacked.Add(caster.container);
            objectivesAttacked.Add(affected[0]);

            var objective = affected[0];

            while (dashCount > 0)
            {
                dashCount--;

                Detect(caster.container, objective.transform.position, Aiming);
                foreach (var item in affected)
                {
                    if(!objectivesAttacked.Contains(item))
                    {
                        objectivesAttacked.Add(item);
                        objective = item;
                        break;
                    }
                }
            }

            objectivesAttacked.RemoveAt(0);
            ObjectiveToAim = objectivesAttacked[0].transform.position;
        }
        else
        {
            Cast();
            return;
        }

        timerToEnd.Reset();
        buttonPress = false;
        caster.abilityControllerMediator -= this;
    }

    public override void OnStayState(CasterEntityComponent param)
    {
        if (buttonPress)
            return;

        FeedBackReference?.Area(FinalMaxRange, FinalMinRange).Direction(Aiming);

        Detect();

        if (affected.Count == 0 || (timerToEnd.total - timerToEnd.current) < triggerBase.cooldownWaitAttack)
            return;

        objectivesAttacked.RemoveAt(0);

        if(objectivesAttacked.Count<=0)
        {
            timerToEnd.Stop();
            Cast(()=>End = true);
            return;
        }

        Cast(() => End = false);

        timerToEnd.Reset();

        ObjectiveToAim = objectivesAttacked[0].transform.position;

        moveEntity.Velocity(Aiming, triggerBase.velocityInDash);

        FeedBackReference?.Area( FinalMaxRange,  FinalMinRange);
    }
}
