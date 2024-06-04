using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;

public class AnimatorController3D : ComponentOfContainer<Entity>
{
    [SerializeField,Tooltip("Set true to change to a elder version of animation system")]
    bool clipAnimation;

    [Header("New version")]

    [SerializeField]
    Animator animator;

    [SerializeField]
    Pictionarys<string, AnimationClip> animations = new Pictionarys<string, AnimationClip>();

    AnimatorOverrideController animatorOverrideController;

    [SerializeField]
    string action1Name, action2Name, actionLoop1Name, actionLoop2Name;

    bool action = true;

    bool loopAction = true;

    string strAction => action ? "Action1" : "Action2";

    string strLoopAction => loopAction ? "LoopAction1" : "LoopAction2";

    [Header("Old version")]

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

    [ContextMenu("Test")]
    void Test()
    {
        ChangeActionAnimation(attackNameAnim);
    }

    public void ChangeActionAnimation(AnimationClip newClip)
    {
        if(action)
        {
            ChangeAnimation(action2Name, newClip);
        }
        else
        {
            ChangeAnimation(action1Name, newClip);
        }

        action = !action;
    }

    void ChangeAnimation(string name, AnimationClip newClip)
    {
        animations.ContainsKey(name, out int index);

        animatorOverrideController[animations.GetPic(index).value] = newClip;

        animations.GetPic(index).value = newClip;
    }

    private void Ia_onMove(Vector3 obj)
    {
        if (clipAnimation)
            animation.Play(moveNameAnim.name);
        else
            animator.SetBool("Move", true);

        animation.transform.forward = Vector3.Lerp(animation.transform.forward, obj, Time.fixedDeltaTime*10);
    }

    private void Ia_onIdle()
    {
        if (clipAnimation)
            animation.Play(idleNameAnim.name);
        else
            animator.SetBool("Move", false);
    }

    private void Ia_onAttack()
    {
        if (clipAnimation)
            animation.Play(attackNameAnim.name);
        else
            animator.SetTrigger(strAction);
    }

    private void Ia_onDeath()
    {
        if (clipAnimation)
            animation.Play(deathNameAnim.name);
        else
            animator.SetTrigger("Death");
    }

    public override void OnEnterState(Entity param)
    {
        if (animation == null)
            return;
        if (clipAnimation)
            ClipAnimation();
        else
            SuperAnimator();

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

    void SuperAnimator()
    {
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        foreach (var clip in animatorOverrideController.animationClips)
        {
            animations.ContainsKey(clip.name, out int index);

            animatorOverrideController[animations.GetPic(index).key] = animations.GetPic(index).value;
        }
    }

    void ClipAnimation()
    {
        idleNameAnim.legacy = true;

        attackNameAnim.legacy = true;

        moveNameAnim.legacy = true;

        deathNameAnim.legacy = true;

        animation.AddClip(idleNameAnim, idleNameAnim.name);

        animation.AddClip(attackNameAnim, attackNameAnim.name);

        animation.AddClip(moveNameAnim, moveNameAnim.name);

        animation.AddClip(deathNameAnim, deathNameAnim.name);
    }

    private void OnValidate()
    {
        if (animator != null)
        {
            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if(!animations.ContainsKey(clip.name, out int index))
                {
                    animations.Add(clip.name, clip);
                    //index = animations.Count - 1;
                }
            }
        }  
    }
}
