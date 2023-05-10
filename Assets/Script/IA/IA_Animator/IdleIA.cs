using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleIA : StateMachineBehaviour
{
    IAAnimator me;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (me == null)
        {
            me = animator.GetComponentInParent<IAAnimator>();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(me.enemy == null)
        {
            animator.Play("Detect");
            return;
        }

        if ((me.enemy.GetTransform().position - animator.transform.position).sqrMagnitude > (me.automatick.radius * me.automatick.radius) / 4)
            animator.Play("Follow");
        else if(me.automatick.cooldown<=0)
            animator.Play("PreAttack");
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

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
