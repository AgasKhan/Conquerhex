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

    public Transform transformModel => controller?.transform;

    [SerializeField]
    bool active = true;

    [SerializeField]
    Animator controller;

    [SerializeField]
    Pictionarys<string, Data> animations = new Pictionarys<string, Data>();

    [SerializeField]
    string[] actionsName;

    AnimatorOverrideController animatorOverrideController;

    List<AnimActionBehaviour> actionBehaviours = new();



    MoveEntityComponent move;

    Vector3 forwardObj = Vector3.forward;

    Timer timerEndAnimationTransition;

    bool loopAction = false;

    const int maxActions = 4;

    int index => ++_index > maxActions-1 ? _index = 0 : _index;

    int _index = 0;

    System.Action endAnimation;

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

    public void SetScaleController(AnimatorUpdateMode _mode = AnimatorUpdateMode.Normal)
    {
        controller.updateMode = _mode;
    }
    /// <summary>
    /// Setea el animator y sus timers para que sean o no Unscaled. TRUE-UNSCALED / FALSE-SCALED
    /// </summary>
    /// <param name="_condition"></param>
    public void SetUnscaleController(bool _condition)
    {
        if(_condition)
        {
            controller.updateMode = AnimatorUpdateMode.UnscaledTime;
            timerEndAnimationTransition.SetUnscaled(true);
        }
        else
        {
            controller.updateMode = AnimatorUpdateMode.Normal;
            timerEndAnimationTransition.SetUnscaled(false);
        }
        
    }

    private void Ia_onMove(Vector3 obj)
    {
        controller.SetBool("Move", true);
    }

    private void Ia_onIdle()
    {
        controller.SetBool("Move", false);
    }

    private void OnCasterAnimation(AnimationInfo.Data obj)
    {
        ChangeActionAnimation(obj);
    }

    private void Ia_onDeath()
    {
         controller.SetTrigger("Death");
    }
    
    public void CancelAllAnimations()
    {
        for (int i = 0; i < maxActions; i++)
        {
            controller.ResetTrigger($"Action{(i + 1)}");
        }

        controller.SetBool("Wait", false);
        controller.SetBool("Move", false);
    }

    public void ChangeActionAnimation(AnimationInfo.Data data)
    {
        controller.SetBool("Mirror", data.mirror);
        controller.SetFloat("ActionMultiply", data.velocity);
        ChangeActionAnimation(data.animationClip, data.defaultAction, data.inLoop);

        endAnimation = null;

        if (data.nextAnimation != null)
        {
            endAnimation = ()=> ChangeActionAnimation(data.nextAnimation);
        }
    }

    public void ChangeActionAnimation(AnimationClip newClip, bool inLoop)
    {
        ChangeActionAnimation(newClip, null, inLoop);
    }

    public void ChangeActionAnimation(AnimationClip newClip, DefaultActions action, bool inLoop = false)
    {
        string name = null;

        int index = (int)action;


        if (index != -1)
            name = actionsName[index];            


        ChangeActionAnimation(newClip, name, inLoop);
    }

    public void ChangeActionAnimation(AnimationClip newClip, string name = null, bool inLoop = false)
    {
        int i = index;

        loopAction = inLoop;

        //
        //ChangeAnimation(actionsName[i],newClip);
        timerEndAnimationTransition.Stop();

        Debug.Log("Change Animation: " + newClip.name);

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

    public void AddActionBehaviours(AnimActionBehaviour actionBehaviour)
    {
        actionBehaviours.Add(actionBehaviour);
        actionBehaviour.onEnter += OnAnimStartAction;
        actionBehaviour.onExit += OnAnimExitAction;
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

        /*
        actionBehaviours = controller.GetBehaviours<AnimActionBehaviour>();

        foreach (var item in actionBehaviours)
        {
            item.onEnter += OnAnimStartAction;
            item.onExit += OnAnimExitAction;
        }
        */
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
        Debug.Log("Fin timer");
        controller.SetBool("Wait", false);
        endAnimation?.Invoke();
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

   

    private void CharacterOnModelView(Vector3 obj)
    {
        obj.y = 0;
        forwardObj = obj;
        enabled = true;
    }

    private void CharacterOnTakeDamage(Damage obj)
    {
        controller.SetTrigger("Hurt");
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

        if (eventMediator == null)
            eventMediator = controller.gameObject.AddComponent<AnimationEventMediator>();

        eventMediator.reference = this;
        /////////

        if (entity.TryGetInContainer<CasterEntityComponent>(out var caster))
        {
            caster.onAnimation += OnCasterAnimation;
            caster.onExitCasting += OnExitCasting;
        }

        if (entity is Character character)
        {
            character.moveStateCharacter.OnActionEnter += MoveStateCharacter_OnActionEnter;
            character.moveStateCharacter.OnActionExit += MoveStateCharacter_OnActionExit;

            character.onModelView += CharacterOnModelView;

            character.onTakeDamage += CharacterOnTakeDamage;

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

        if (entity is Character character)
        {
            character.moveStateCharacter.OnActionEnter -= MoveStateCharacter_OnActionEnter;
            character.moveStateCharacter.OnActionExit -= MoveStateCharacter_OnActionExit;

            character.onModelView -= CharacterOnModelView;

            character.onTakeDamage -= CharacterOnTakeDamage;

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
