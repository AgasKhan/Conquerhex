using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ComponentsAndContainers;

public partial class AnimatorController : ComponentOfContainer<Entity>
{
    [System.Serializable]
    public class Data
    {
        public bool canChange = true;
        public AnimationClip clip;
    }

    [System.Serializable]
    public enum DefaultActions
    {
        NoDefault = -1,
        Cast = 0,
        Attack=0,
        SpecialCast1=1,
        SpecialAttack1=1,
        SpecialCast2 = 3,
        SpecialAttack2 = 3,
        SpecialCast3 = 2,
        SpecialAttack3 = 2,
    }

    public event System.Action<AnimatorStateInfo> onEnterAnim;
    public event System.Action<AnimatorStateInfo> onExitAnim;

    public bool isPlaying { get; private set; }

    public Vector3 forwardModel => controller.transform.forward;
    public Quaternion rotationModel => controller.transform.rotation;

    [SerializeField]
    bool active = true;

    [SerializeField]
    Animator controller;

    [SerializeField]
    Pictionarys<string, Data> animations = new Pictionarys<string, Data>();

    [SerializeField]
    string[] actionsName;



    AnimatorOverrideController animatorOverrideController;

    AnimActionBehaviour[] actionBehaviours;

    MoveEntityComponent move;

    Vector3 forwardObj;

    Timer timerEndAnimationTransition;

    bool loopAction = false;

    const int maxActions = 4;

    int index => ++_index > maxActions-1 ? _index = 0 : _index;

    int _index = 0;

    string strAction => "Action" + (_index+1);

    #region Test

    [SerializeField]
    AnimationClip animatorClipTest;

    [ContextMenu("Test")]
    void Test()
    {
        ChangeActionAnimation(animatorClipTest);
    }

    [ContextMenu("TestLoop")]
    void TestLoop()
    {
        ChangeActionAnimation(animatorClipTest, true);
    }

    [ContextMenu("TestDefaultCast")]
    void TestDefaultCast()
    {
        ChangeActionAnimation(animatorClipTest, DefaultActions.Cast);
    }

    #endregion

    private void Ia_onMove(Vector3 obj)
    {
        controller.SetBool("Move", true);

        if (obj != Vector3.zero )
            forwardObj = Vector3.Slerp(forwardObj, obj, Time.fixedDeltaTime*10);

        enabled = true;
    }

    private void Ia_onIdle()
    {
        controller.SetBool("Move", false);
    }

    private void Ia_onAiming(Vector3 obj)
    {
        if (obj != Vector3.zero)
            forwardObj =  obj;
        else
            return;

        forwardObj.y = 0;

        enabled = true;
    }

    private void OnCasterAnimation(AnimationInfo.Data obj)
    {
        ChangeActionAnimation(obj);
    }

    private void Ia_onDeath()
    {
         controller.SetTrigger("Death");
    }
    
    void CancelAllAnimations()
    {
        for (int i = 0; i < maxActions; i++)
        {
            controller.ResetTrigger($"Action{(i + 1)}");
        }

        controller.SetBool("Wait", false);
    }

    void ChangeActionAnimation(AnimationInfo.Data data)
    {
        controller.SetBool("Mirror", data.mirror);
        controller.SetFloat("ActionMultiply", data.velocity);
        ChangeActionAnimation(data.animationClip, data.defaultAction, data.inLoop);
    }

    void ChangeActionAnimation(AnimationClip newClip, bool inLoop)
    {
        ChangeActionAnimation(newClip, null, inLoop);
    }

    void ChangeActionAnimation(AnimationClip newClip, DefaultActions action, bool inLoop = false)
    {
        string name = null;

        int index = (int)action;


        if (index != -1)
            name = actionsName[index];            


        ChangeActionAnimation(newClip, name, inLoop);
    }

    void ChangeActionAnimation(AnimationClip newClip, string name = null, bool inLoop = false)
    {
        int i = index;

        loopAction = inLoop;

        //
        //ChangeAnimation(actionsName[i],newClip);
        timerEndAnimationTransition.Stop();

        if(name!=null && animations.ContainsKey(name, out int indexPic))
        {
            var pic = animations.GetPic(indexPic);

            if (!pic.value.canChange)
            {
                newClip = pic.value.clip;
            }
        }

        if(animations.ContainsKey(actionsName[i], out indexPic))
        {
            var pic = animations.GetPic(indexPic);

            animatorOverrideController[pic.key] = newClip;

            controller.SetBool("Wait", false);

            controller.SetTrigger(strAction);
        }
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

        actionBehaviours = controller.GetBehaviours<AnimActionBehaviour>();

        foreach (var item in actionBehaviours)
        {
            item.onEnter += OnAnimStartAction;
            item.onExit += OnAnimExitAction;
        }
    }

    private void OnAnimExitAction(AnimatorStateInfo obj)
    {
        isPlaying = false;
        onExitAnim?.Invoke(obj);
    }

    private void OnAnimStartAction(AnimatorStateInfo obj)
    {
        isPlaying = true;

        if (!loopAction)
            timerEndAnimationTransition.Set(obj.length);

        onEnterAnim?.Invoke(obj);
    }

    private void TimerEndAnimation()
    {
        controller.SetBool("Wait", false);
    }

    #if UNITY_EDITOR

    private void OnValidate()
    {
        if (controller != null)
        {
            foreach (var clip in controller.runtimeAnimatorController.animationClips)
            {
                if (!animations.ContainsKey(clip.name, out int index))
                {
                    animations.Add(clip.name, new Data() { clip = clip, canChange = true });
                    //index = animations.Count - 1;
                }
            }
        }
    }

    #endif

    public override void OnEnterState(Entity entity)
    {
        if (controller == null || !active)
        {
            controller.SetActiveGameObject(false);
            return;
        }

        SuperAnimator();

        timerEndAnimationTransition = TimersManager.Create(1, TimerEndAnimation).Stop();

        /////////
        ///Se quitara en proximas updates
        var eventMediator = controller.gameObject.GetComponent<AnimationEventMediator>();

        if(eventMediator == null)
            eventMediator = controller.gameObject.AddComponent<AnimationEventMediator>();

        eventMediator.reference = this;
        /////////

        if(entity.TryGetInContainer<CasterEntityComponent>(out var caster))
        {
            caster.onAnimation += OnCasterAnimation;
            caster.onExitCasting += OnExitCasting;
        }

        if (entity.TryGetInContainer<AimingEntityComponent>(out var aiming))
        {
            aiming.onAimingXZ += Ia_onAiming;
        }

        if(entity is Character character)
        {
            character.moveStateCharacter.OnActionEnter += MoveStateCharacter_OnActionEnter;
            character.moveStateCharacter.OnActionExit += MoveStateCharacter_OnActionExit;
            move = character.move;
            move.onIdle += Ia_onIdle;
            MoveStateCharacter_OnActionEnter();
        }
        else if (entity.TryGetInContainer<MoveEntityComponent>(out move))
        {
            move.onIdle += Ia_onIdle;
            move.onMove += Ia_onMove;
        }

        entity.health.death += Ia_onDeath;
    }

    private void OnExitCasting(Ability obj)
    {
        CancelAllAnimations();
    }

    private void MoveStateCharacter_OnActionEnter()
    {
        //move.onIdle += Ia_onIdle;
        move.onMove += Ia_onMove;
    }

    private void MoveStateCharacter_OnActionExit()
    {
        //move.onIdle -= Ia_onIdle;
        move.onMove -= Ia_onMove;
        //Ia_onIdle();
    }


    private void Update()
    {
        controller.transform.forward = Vector3.Slerp(controller.transform.forward, forwardObj, Time.deltaTime * 10);
        if ((controller.transform.forward - forwardObj).sqrMagnitude < 0.01f)
            enabled = false;
    }

    public override void OnStayState(Entity param)
    {
    }

    public override void OnExitState(Entity entity)
    {
        if (entity.TryGetInContainer<CasterEntityComponent>(out var caster))
        {
            caster.onAnimation -= OnCasterAnimation;
            caster.onExitCasting -= OnExitCasting;
        }

        if (entity.TryGetInContainer<AimingEntityComponent>(out var aiming))
        {
            aiming.onAimingXZ -= Ia_onAiming;
        }

        if (entity is Character character)
        {
            character.moveStateCharacter.OnActionEnter -= MoveStateCharacter_OnActionEnter;
            character.moveStateCharacter.OnActionExit -= MoveStateCharacter_OnActionExit;
            move = character.move;
            move.onIdle -= Ia_onIdle;
            MoveStateCharacter_OnActionExit();
        }
        else if (entity.TryGetInContainer<MoveEntityComponent>(out move))
        {
            move.onIdle -= Ia_onIdle;
            move.onMove -= Ia_onMove;
        }

        entity.health.death -= Ia_onDeath;
    }
}
