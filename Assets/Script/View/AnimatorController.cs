using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;

public class AnimatorController : ComponentOfContainer<Entity>
{
    [SerializeField]
    Animator animator;

    [SerializeField]
    string attackNameAnim = "Attack";

    [SerializeField]
    string moveNameAnim = "Move";

    [SerializeField]
    string deathNameAnim = "Death";

    private void Ia_onMove(Vector2 obj)
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
        animator = param.GetComponentInChildren<Animator>();

        container.GetInContainer<CasterEntityComponent>().onAttack += Ia_onAttack;

        container.GetInContainer<MoveEntityComponent>().move.onIdle += Ia_onIdle;

        container.GetInContainer<MoveEntityComponent>().move.onMove += Ia_onMove;

        container.health.death += Ia_onDeath;
    }

    public override void OnStayState(Entity param)
    {
        throw new System.NotImplementedException();
    }

    public override void OnExitState(Entity param)
    {
        throw new System.NotImplementedException();
    }
}
