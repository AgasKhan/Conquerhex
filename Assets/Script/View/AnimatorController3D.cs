using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;

public class AnimatorController3D : ComponentOfContainer<Entity>
{
    [SerializeField]
    Animator animator;

    [SerializeField]
    new Animation animation;

    [SerializeField]
    AnimationClip idleNameAnim;

    [SerializeField]
    AnimationClip attackNameAnim;

    [SerializeField]
    AnimationClip moveNameAnim;

    [SerializeField]
    AnimationClip deathNameAnim;

    private void Ia_onMove(Vector3 obj)
    {
        animation.Play(moveNameAnim.name);

        animation.transform.forward = Vector3.Lerp(animation.transform.forward, obj, Time.fixedDeltaTime*10);
    }

    private void Ia_onIdle()
    {
        animation.Play(idleNameAnim.name);
    }

    private void Ia_onAttack()
    {
        animation.Play(attackNameAnim.name);
    }
    private void Ia_onDeath()
    {
        animation.Play(deathNameAnim.name);
    }

    public override void OnEnterState(Entity param)
    {
        if (animation == null)
            return;

        idleNameAnim.legacy = true;

        attackNameAnim.legacy = true;

        moveNameAnim.legacy = true;

        deathNameAnim.legacy = true;

        animation.AddClip(idleNameAnim, idleNameAnim.name);

        animation.AddClip(attackNameAnim, attackNameAnim.name);

        animation.AddClip(moveNameAnim, moveNameAnim.name);

        animation.AddClip(deathNameAnim, deathNameAnim.name);

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

        idleNameAnim.legacy = false;

        attackNameAnim.legacy = false;

        moveNameAnim.legacy = false;

        deathNameAnim.legacy = false;
    }
}
