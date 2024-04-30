using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/DashToEntityUpTriggerControllerBase")]
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
        if (End)
        {
            return;
        }

        buttonPress = true;
        
        FeedBackReference?.DotAngle(Dot);
        objectivesAttacked.Clear();
        dashCount = triggerBase.dashCount;
    }

    public override void ControllerUp(Vector2 dir, float button)
    {
        if (!onCooldownTime)
        {
            End = true;
            cooldown.Reset();
            return;
        }
        
        cooldown.Reset();

        if (affected != null && affected.Count != 0 && caster.TryGetComponent<MoveEntityComponent>(out moveEntity))
        {
            moveEntity.Velocity((affected[0].transform.position - caster.transform.position).normalized , triggerBase.velocityInDash);
            
            dashCount--;
            objectivesAttacked.Add(caster.container);
            objectivesAttacked.Add(affected[0]);

            var objective = affected[0];

            while (dashCount > 0)
            {
                dashCount--;

                Detect(caster.container, objective.transform.position ,0, FinalMaxRange*1.5f);
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
        }
        else
        {
            Cast();
            return;
        }

        timerToEnd.Reset();
        buttonPress = false;
    }

    public override void OnStayState(CasterEntityComponent param)
    {
        if (buttonPress)
            return;

        FeedBackReference?.Area(originalScale * FinalMaxRange * 1f / 2, originalScale * FinalMinRange * 1f / 2);

        Detect(0, FinalMaxRange * 1f / 2, originalScale * FinalMinRange * 1f / 2);

        if (affected.Count == 0 || (timerToEnd.total - timerToEnd.current) < triggerBase.cooldownWaitAttack)
            return;

        objectivesAttacked.RemoveAt(0);

        FeedBackReference?.Attack();

        Cast();

        if(objectivesAttacked.Count<=0)
        {
            timerToEnd.Stop();
            End = true;
            return;
        }


        timerToEnd.Reset();

        End = false;

        Aiming = (objectivesAttacked[objectivesAttacked.Count-1].transform.position - caster.transform.position).normalized;

        moveEntity.Velocity(Aiming, triggerBase.velocityInDash);

        FeedBackReference?.Area(originalScale * FinalMaxRange, originalScale * FinalMinRange).Direction(Aiming);
    }
}
