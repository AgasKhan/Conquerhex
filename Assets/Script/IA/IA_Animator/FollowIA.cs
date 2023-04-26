using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowIA : StateMachineBehaviour
{
    MoveAbstract me;
    Character character;
    Seek seek;


    [SerializeField] float _viewRadius;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        character = GameManager.instance.player.GetComponent<Character>();

        if (me == null)
            me = animator.GetComponent<MoveAbstract>();

        seek = character.GetComponent<Seek>();

        me.Velocity(5f);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        me.Acelerator((character.transform.position - animator.transform.position));

        if ((character.transform.position - animator.transform.position).magnitude <= animator.GetFloat("MinDistance") )
            animator.SetTrigger("Ataque");
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

}
