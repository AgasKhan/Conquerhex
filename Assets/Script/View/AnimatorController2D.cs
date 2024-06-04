using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;

public class AnimatorController2D : ComponentOfContainer<Entity>
{
    [SerializeField]
    bool active = true;

    [SerializeField]
    Animator animator;

    [SerializeField]
    string attackNameAnim = "Attack";

    [SerializeField]
    string moveNameAnim = "Move";

    [SerializeField]
    string deathNameAnim = "Death";

    private void Ia_onMove(Vector3 obj)
    {
        animator.SetBool(moveNameAnim, true);
    }

    private void Ia_onIdle()
    {
        animator.SetBool(moveNameAnim, false);
    }

    private void Ia_onAttack()
    {
        animator.SetTrigger(attackNameAnim);
    }
    private void Ia_onDeath()
    {
        animator.SetTrigger(deathNameAnim);
    }

    public override void OnEnterState(Entity param)
    {
        if (animator==null || !active)
        {
            animator.SetActiveGameObject(false);
            return;
        }

        container.GetInContainer<CasterEntityComponent>().onAttack += Ia_onAttack;

        container.GetInContainer<MoveEntityComponent>().onIdle += Ia_onIdle;

        container.GetInContainer<MoveEntityComponent>().onMove += Ia_onMove;

        container.health.death += Ia_onDeath;
    }

    public override void OnStayState(Entity param)
    {
        //throw new System.NotImplementedException();
    }

    public override void OnExitState(Entity param)
    {
        //throw new System.NotImplementedException();
    }
}
