using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle_PatrolEnemy : StateMachineBehaviour
{
    float _elapsedTime;
    PatrolEnemy _defaultEnemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _defaultEnemy = animator.GetComponent<PatrolEnemy>();
        _elapsedTime = _defaultEnemy.GetWaitingTime();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // patrol own paths for every waiting time
        if(_elapsedTime <= 0)
        {
            animator.SetTrigger("StartPatrol");
        }
        _elapsedTime -= Time.deltaTime;

    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

    }
}
