using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackReady_Enemy : StateMachineBehaviour
{
    EnemyAttack _enemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(animator.TryGetComponent<EnemyAttack>(out  _enemy))
        {
//            _enemy.ChangeWaveColorToReadyColor();
            Debug.Log("ReadyAttack");
        }
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
