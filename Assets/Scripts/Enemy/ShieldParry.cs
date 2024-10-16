using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyState;

public class ShieldParry : MonoBehaviour, IKnockback
{
    public void Knockback(Vector2 knockBackDirection, StateMachine playerState)
    {
        Debug.Log("knockback!");
        EnemyMovement enemyMovement = GetComponentInParent<EnemyMovement>();
        ShielderAttack shielderAttack = GetComponentInParent<ShielderAttack>();
        
        // stop player tackle
        if (playerState is PlayerState.ShortAttackState)
        {
            PlayerState.ShortAttackState shortAttackState = (PlayerState.ShortAttackState)playerState;

            shortAttackState.IsPrevented = true;
        }

        // knockback
        if (enemyMovement != null && shielderAttack != null)
        {
            Debug.Log("I goona go back");
            shielderAttack.KnockbackDirection = knockBackDirection;
            enemyMovement.CurrentState.SwitchState(enemyMovement.gameObject, ref enemyMovement.CurrentState, new KnockbackState());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        IParriable parriable;
        if (collision.gameObject.TryGetComponent<IParriable>(out parriable))
        {
            Debug.Log($"parriable! I'm {gameObject.name}");
            parriable.IsParried = true;
        }
    }
}
