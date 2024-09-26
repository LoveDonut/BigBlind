using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chase_Enemy : StateMachineBehaviour
{
    Enemy _enemy;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.TryGetComponent<Enemy>(out _enemy))
        {
            if (!_enemy.IsInAttackRange())
            {
                _enemy.StartMove();
                _enemy.Chase();
            }
        }
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.TryGetComponent<Enemy>(out _enemy))
        {
            if (_enemy.IsInAttackRange())
            {
                animator.SetTrigger("StartReady");
                return;
            }
            _enemy.Chase();
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.TryGetComponent<Enemy>(out _enemy))
        {
//            animator.ResetTrigger("StartReady");
            _enemy.StopMove();
        }
    }
}
