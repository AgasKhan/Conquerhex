using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectIA : StateMachineBehaviour
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
        if(me.enemy != null)
            animator.Play("Idle");
    }

}
