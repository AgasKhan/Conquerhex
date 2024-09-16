using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;

public partial class AnimatorController : ComponentOfContainer<Entity>
{
    [System.Serializable]
    public class AnimationData
    {
        public bool canChange = true;
        public AnimationClip clip;
    }

    public Dictionary<string, System.Action> animationEventMediator = new Dictionary<string, System.Action>();

    [SerializeField]
    bool active = true;

    [SerializeField]
    public Animator controller;

    [SerializeField]
    Pictionarys<string, AnimationData> animations = new Pictionarys<string, AnimationData>();

    [SerializeField]
    string action1Name,
        action2Name,
        actionLoop1NameMiddle,
        actionLoop2NameMiddle;


    AnimatorOverrideController animatorOverrideController;

    bool action = true;

    bool loopAction = true;

    string strAction => action ? "Action1" : "Action2";

    string strLoopAction => loopAction ? "ActionLoop1" : "ActionLoop2";

    MoveEntityComponent move;

    Vector3 forward;

    private void Ia_onMove(Vector3 obj)
    {
        controller.SetBool("Move", true);

        if (obj != Vector3.zero )
            forward = Vector3.Slerp(forward, obj, Time.fixedDeltaTime*10);

        enabled = true;
    }

    private void Ia_onIdle()
    {
        controller.SetBool("Move", false);
    }

    private void Ia_onAiming(Vector3 obj)
    {
        if (obj != Vector3.zero)
            forward =  obj;
        else
            return;

        forward.y = 0;

        enabled = true;
    }

    private void Ia_PreCast(Ability ability)
    {
        /*
        switch (ability.state)
        {
            case Ability.State.middle:

                break;

            case Ability.State.start:

                controller.SetBool(strLoopAction, false);

                if (ability.animationCastMiddle == null && ability.animationCastExit == null)
                {

                    if(ability.WaitAnimations)
                        animationEventMediator["Cast"] = ()=> ability.Cast();
                    else
                        animationEventMediator["Cast"] = null;

                    ChangeActionAnimation(ability.animationCastStart);
                }
                else
                {
                    ChangeLoopActionAnimation(ability.animationCastStart, ability.animationCastMiddle, ability.animationCastExit);
                }
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
        */
    }

    private void Ia_onDeath()
    {
         controller.SetTrigger("Death");
    }

    void ChangeLoopActionAnimation(AnimationClip newClipStart, AnimationClip newClipMiddle, AnimationClip newClipEnd)
    {
        if (loopAction)
        {
            ChangeActionAnimation(newClipStart);
            ChangeAnimation(actionLoop2NameMiddle, newClipMiddle);
            ChangeActionAnimation(newClipEnd);

        }
        else
        {
            ChangeActionAnimation(newClipStart);
            ChangeAnimation(actionLoop1NameMiddle, newClipMiddle);
            ChangeActionAnimation(newClipEnd);
        }

        loopAction = !loopAction;

        controller.SetBool(strLoopAction, true);
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

        controller.SetTrigger(strAction);
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

        var eventMediator = controller.gameObject.GetComponent<AnimationEventMediator>();

        if(eventMediator == null)
            eventMediator = controller.gameObject.AddComponent<AnimationEventMediator>();

        eventMediator.reference = this;

        if(container.TryGetInContainer<CasterEntityComponent>(out var caster))
        {
            caster.onPreCast += Ia_PreCast;
        }

        if (container.TryGetInContainer<AimingEntityComponent>(out var aiming))
        {
            aiming.onAimingXZ += Ia_onAiming;
        }

        if(container is Character character)
        {
            character.moveStateCharacter.OnActionEnter += MoveStateCharacter_OnActionEnter;
            character.moveStateCharacter.OnActionExit += MoveStateCharacter_OnActionExit;
            move = character.move;
            MoveStateCharacter_OnActionEnter();
        }
        else if (container.TryGetInContainer<MoveEntityComponent>(out move))
        {
            move.onIdle += Ia_onIdle;
            move.onMove += Ia_onMove;
        }

        container.health.death += Ia_onDeath;
    }


    private void MoveStateCharacter_OnActionEnter()
    {
        move.onIdle += Ia_onIdle;
        move.onMove += Ia_onMove;
    }


    private void MoveStateCharacter_OnActionExit()
    {
        move.onIdle -= Ia_onIdle;
        move.onMove -= Ia_onMove;
    }


    private void Update()
    {
        controller.transform.forward = Vector3.Slerp(controller.transform.forward, forward, Time.deltaTime * 10);
        if ((controller.transform.forward - forward).sqrMagnitude < 0.01f)
            enabled = false;
    }

    public override void OnStayState(Entity param)
    {
        //controller.transform.forward = Vector3.Slerp(controller.transform.forward, forward, Time.deltaTime * 10);
    }

    public override void OnExitState(Entity param)
    {

    }
}
