using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Patrol_DefaultEnemy : StateMachineBehaviour
{
    NavMeshAgent _navMeshAgent;
    DefaultEnemy _defualteEnemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _navMeshAgent = animator.GetComponent<NavMeshAgent>();
        _defualteEnemy = animator.GetComponent<DefaultEnemy>();
        _navMeshAgent.SetDestination(_defualteEnemy.GetDestination().position);
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(Vector3.Distance(animator.transform.position, _navMeshAgent.destination) < 0.1f)
        {
            animator.SetTrigger("TrigerIdle");
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
