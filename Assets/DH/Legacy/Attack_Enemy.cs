using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack_Enemy : StateMachineBehaviour
{
    EnemyAttack _enemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("Attack");
        animator.SetTrigger("StartChase");
    }

    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
}
