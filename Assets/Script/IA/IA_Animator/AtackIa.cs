using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtackIa : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    IAAnimator me;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (me == null)
        {
            me = animator.GetComponentInParent<IAAnimator>();
        }

        me.automatick.Attack();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    /*
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
    */
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
}
