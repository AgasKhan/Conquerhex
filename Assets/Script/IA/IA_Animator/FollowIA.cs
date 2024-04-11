using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowIA : StateMachineBehaviour
{
    MoveEntityComponent meMove;
    IAAnimator me;
    

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (me == null)
        {
            meMove = animator.GetComponentInParent<MoveEntityComponent>();
            me = animator.GetComponentInParent<IAAnimator>();
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (me.enemy == null || (me.enemy.transform.position - animator.transform.position).sqrMagnitude < (me.automatick.radius * me.automatick.radius) / 4)
        {
            animator.Play("Idle");
            return;
        }

        meMove.ControllerPressed((me.enemy.transform.position - animator.transform.position).normalized, 0);
    }

}
