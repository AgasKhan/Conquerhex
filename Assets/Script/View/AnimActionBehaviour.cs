using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimActionBehaviour : StateMachineBehaviour
{
    public event System.Action<AnimatorStateInfo> onEnter;
    public event System.Action<AnimatorStateInfo> onExit;

    AnimatorController animatorController; 

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(animatorController==null)
        {
            animatorController = animator.GetComponentInParent<AnimatorController>();
            animatorController.AddActionBehaviours(this);
        }

        animator.SetBool("Wait", true);
        onEnter?.Invoke(stateInfo);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{

    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        onExit?.Invoke(stateInfo);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
