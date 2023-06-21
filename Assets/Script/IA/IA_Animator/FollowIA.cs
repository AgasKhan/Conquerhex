using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowIA : StateMachineBehaviour
{
    MoveAbstract meMove;
    IAAnimator me;
    

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (me == null)
        {
            meMove = animator.GetComponentInParent<MoveAbstract>();
            me = animator.GetComponentInParent<IAAnimator>();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (me.enemy == null || ((me.enemy as Component).transform.position - animator.transform.position).sqrMagnitude < (me.automatick.radius * me.automatick.radius) / 4)
        {
            animator.Play("Idle");
            return;
        }

        meMove.ControllerPressed(((me.enemy as Component).transform.position - animator.transform.position).normalized, 0);
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

}
