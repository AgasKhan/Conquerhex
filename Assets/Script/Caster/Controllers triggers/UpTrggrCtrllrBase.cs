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

    Vector3 aiming;
    Vector3 Aiming() => aiming;

    Character character;

    private void MoveEventMediator_eventPress(Vector2 arg1, float arg2)
    {
        if (arg1 != Vector2.zero)
        {
            aiming = arg1.Vect2To3XZ(0);
            character.OnModelView(aiming);
        }
    }

    public override void OnEnterState(CasterEntityComponent param)
    {
        base.OnEnterState(param);

        if((triggerBase?.aimingToMove ?? false) && param.container is Character character)
        {
            this.character = character;

            character.aimingEventMediator.DesuscribeController(character.aiming);

            character.moveEventMediator.eventPress += MoveEventMediator_eventPress;

            aiming = character.moveEventMediator.dir.Vect2To3XZ(0);

            if (aiming != Vector3.zero)
                character.OnModelView(aiming);

            ability.alternativeAiming = Aiming;
        }
    }

    public override void OnExitState(CasterEntityComponent param)
    {
        if ((triggerBase?.aimingToMove ?? false) && param.container is Character character)
        {
            character.aimingEventMediator.SuscribeController(character.aiming);

            character.moveEventMediator.eventPress -= MoveEventMediator_eventPress;

            ability.alternativeAiming = null;
        }

        base.OnExitState(param);
    }


    public override void ControllerDown(Vector2 dir, float tim)
    {
    }

    //Durante, al mantener y moverlo
    public override void ControllerPressed(Vector2 dir, float button)
    {

        ability.FeedbackDetect();

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

