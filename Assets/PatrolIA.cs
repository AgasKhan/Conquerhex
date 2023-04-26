using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolIA : StateMachineBehaviour
{
    MoveAbstract move;
    Vector3 myPos;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        myPos = move.transform.position;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        move.Acelerator(MyPos());
        
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }

    Vector3 MyPos()
    {
        Vector3 roaming = new Vector3(Random.Range(1f, 1f), Random.Range(1f, 1f)).normalized;
        return myPos + roaming * Random.Range(10f, 70f);
    }
}
