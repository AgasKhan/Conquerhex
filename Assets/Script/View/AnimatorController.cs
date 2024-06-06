using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;

public class AnimatorController : ComponentOfContainer<Entity>
{
    [System.Serializable]
    public class AnimationData
    {
        public bool canChange = true;
        public AnimationClip clip;
    }

    [SerializeField]
    bool active = true;

    [SerializeField]
    Animator controller;

    [SerializeField]
    Pictionarys<string, AnimationData> animations = new Pictionarys<string, AnimationData>();

    AnimatorOverrideController animatorOverrideController;

    [SerializeField]
    string action1Name, 
        action2Name, 

        actionLoop1NameStart, 
        actionLoop1NameMiddle, 
        actionLoop1NameEnd, 

        actionLoop2NameStart, 
        actionLoop2NameMiddle, 
        actionLoop2NameEnd;

    bool action = true;

    bool loopAction = true;

    string strAction => action ? "Action1" : "Action2";

    string strLoopAction => loopAction ? "ActionLoop1" : "ActionLoop2";

    private void Ia_onMove(Vector3 obj)
    {
        controller.SetBool("Move", true);

        controller.transform.forward = Vector3.Lerp(controller.transform.forward, obj, Time.fixedDeltaTime*10);
    }

    private void Ia_onIdle()
    {
        controller.SetBool("Move", false);
    }

    private void Ia_onAttack(Ability ability)
    {
        if(ability.Aiming!=Vector3.zero)
            controller.transform.forward = ability.Aiming;

        switch (ability.state)
        {
            case Ability.State.start:

                controller.SetBool(strLoopAction, false);

                if (ability.animationCastMiddle == null && ability.animationCastExit == null)
                {
                    ChangeActionAnimation(ability.animationCastStart);
                    controller.SetTrigger(strAction);
                }
                else
                {
                    ChangeLoopActionAnimation(ability.animationCastStart, ability.animationCastMiddle, ability.animationCastExit);
                    controller.SetBool(strLoopAction, true);
                }
                break;


            case Ability.State.middle:

                break;


            case Ability.State.end:

                if (ability.animationCastMiddle == null && ability.animationCastExit == null)
                {

                }
                else
                {
                    controller.SetBool(strLoopAction, false);
                }
                break;
        }
    }

    private void Ia_onDeath()
    {
         controller.SetTrigger("Death");
    }

    void ChangeLoopActionAnimation(AnimationClip newClipStart, AnimationClip newClipMiddle, AnimationClip newClipEnd)
    {
        if (loopAction)
        {
            ChangeAnimation(actionLoop2NameStart, newClipStart);
            ChangeAnimation(actionLoop2NameMiddle, newClipMiddle);
            ChangeAnimation(actionLoop2NameEnd, newClipEnd);
        }
        else
        {
            ChangeAnimation(actionLoop1NameStart, newClipStart);
            ChangeAnimation(actionLoop1NameMiddle, newClipMiddle);
            ChangeAnimation(actionLoop1NameEnd, newClipEnd);
        }

        loopAction = !loopAction;
    }

    void ChangeActionAnimation(AnimationClip newClip)
    {
        if (action)
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

        var pic = animations.GetPic(index);

        if (!pic.value.canChange)
            return;

        animatorOverrideController[pic.key] = newClip;

        pic.value.clip = newClip;
    }

    void SuperAnimator()
    {
        animatorOverrideController = new AnimatorOverrideController(controller.runtimeAnimatorController);
        controller.runtimeAnimatorController = animatorOverrideController;

        foreach (var clip in animatorOverrideController.animationClips)
        {
            animations.ContainsKey(clip.name, out int index);

            animatorOverrideController[animations.GetPic(index).key] = animations.GetPic(index).value.clip;
        }
    }

    private void OnValidate()
    {
        if (controller != null)
        {
            foreach (var clip in controller.runtimeAnimatorController.animationClips)
            {
                if (!animations.ContainsKey(clip.name, out int index))
                {
                    animations.Add(clip.name, new AnimationData() { clip = clip, canChange = true });
                    //index = animations.Count - 1;
                }
            }
        }
    }

    public override void OnEnterState(Entity param)
    {
        if (controller == null || !active)
        {
            controller.SetActiveGameObject(false);
            return;
        }

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
    }
}
