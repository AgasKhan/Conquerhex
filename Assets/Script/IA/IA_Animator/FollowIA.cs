using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowIA : StateMachineBehaviour
{
    MoveAbstract meMove;
    IAAnimator me;
    Timer timer;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (me == null)
        {
            meMove = animator.GetComponentInParent<MoveAbstract>();
            me = animator.GetComponentInParent<IAAnimator>();
            timer = TimersManager.Create(2);
        }
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (me.enemy == null)
        {
            animator.SetTrigger("Idle");
            return;
        }

        var sqrmagnitude = (me.enemy.GetTransform().position - animator.transform.position).sqrMagnitude;


        if (sqrmagnitude > (me.automatick.radius * me.automatick.radius)/4)
            meMove.ControllerPressed((me.enemy.GetTransform().position - animator.transform.position).normalized , 0);

        if (sqrmagnitude <= (me.automatick.radius * me.automatick.radius) && timer.Chck && me.automatick.timerToAttack.Chck)
        {
            timer.Reset();
            animator.SetTrigger("Attack");
        }
            
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

}
