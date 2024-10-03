using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Trigger/UpTriggerControllerBase")]
public class UpTrggrCtrllrBase : TriggerControllerBase
{
    public bool aimingToMove;

    protected override System.Type SetItemType()
    {
        return typeof(UpTrggrCtrllr);
    }
}

/// <summary>
/// Controlador que ejecuta el ataque cuando se suelta el boton de la habilidad
/// </summary>
public class UpTrggrCtrllr : TriggerController
{
    new UpTrggrCtrllrBase triggerBase=> base.triggerBase as UpTrggrCtrllrBase;

    private void MoveEventMediator_eventPress(Vector2 arg1, float arg2)
    {
        if(arg1!=Vector2.zero)
            Aiming2D = arg1;
    }

    public override void OnEnterState(CasterEntityComponent param)
    {
        base.OnEnterState(param);

        if((triggerBase?.aimingToMove ?? false) && param.container is Character character)
        {
            Aiming2D = character.move.direction.Vect3To2XZ();

            character.moveEventMediator.eventPress += MoveEventMediator_eventPress;

            character.aimingEventMediator.DesuscribeController(character.aiming);
        }
    }

    public override void OnExitState(CasterEntityComponent param)
    {
        if ((triggerBase?.aimingToMove ?? false) && param.container is Character character)
        {
            character.moveEventMediator.eventPress -= MoveEventMediator_eventPress;

            character.aimingEventMediator.SuscribeController(character.aiming);
        }

        base.OnExitState(param);
    }


    public override void ControllerDown(Vector2 dir, float tim)
    {
    }

    //Durante, al mantener y moverlo
    public override void ControllerPressed(Vector2 dir, float button)
    {
        //Aiming = Vector3.Lerp(Aiming, dir.Vect2To3XZ(0), Time.deltaTime*10);
        if(!(triggerBase?.aimingToMove ?? false))
            Aiming2D = dir;            

        FeedBackReference?.Area(FinalMaxRange).Direction(AimingXZ).Angle(Angle);

        Detect();
    }

    //Despues, al sotarlo
    public override void ControllerUp(Vector2 dir, float button)
    {
        Cast();

        if(affected!=null && affected.Count>0 && !(triggerBase?.aimingToMove ?? false))
            ObjectiveToAim = affected[0].transform.position;

        caster.abilityControllerMediator -= this;
    }

}

