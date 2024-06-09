using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/UpTriggerControllerBase")]
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

    public override void ControllerDown(Vector2 dir, float button)
    {
        if (!onCooldownTime)
        {
            End = true;
            return;
        }

        //FeedBackReference?.Area(out originalScale);
    }

    //Durante, al mantener y moverlo
    public override void ControllerPressed(Vector2 dir, float button)
    {
        if (!onCooldownTime)
        {
            End = true;
            return;
        }

        //Aiming = Vector3.Lerp(Aiming, dir.Vect2To3XZ(0), Time.deltaTime*10);
        if(!(triggerBase?.aimingToMove ?? false))
            Aiming = dir.Vect2To3XZ(0);
        else
        {
            if(caster.TryGetInContainer(out MoveEntityComponent move))
            {
                Aiming = move.direction;
            }
        }
            

        FeedBackReference?.Area(FinalMaxRange).Direction(Aiming).Angle(Angle);

        Detect();
    }

    //Despues, al sotarlo
    public override void ControllerUp(Vector2 dir, float button)
    {
        if (!onCooldownTime)
        {
            End = true;
            return;
        }

        //comienza a bajar el cooldown

        Cast();

        FeedBackReference?.Attack();

        caster.abilityControllerMediator -= this;
    }
}

