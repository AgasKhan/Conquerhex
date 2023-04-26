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
        if(me == null)
           me.GetComponent<MoveAbstract>();

        seek = seek.GetComponent<Seek>();
        character = character.GetComponent<Character>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if((me.transform.position - character.transform.position).sqrMagnitude <= _viewRadius * _viewRadius)
            me.Acelerator(seek.Calculate(me.Director(character.transform.position)));
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

}
