using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EnemyState;

// made by Daehui
public class ShieldParry : MonoBehaviour, IKnockback
{
    public void Knockback(Vector2 knockBackDirection, GameObject gameObject)
    {
        PlayerMovement playerMovement = gameObject.GetComponent<PlayerMovement>();
        EnemyMovement enemyMovement = GetComponentInParent<EnemyMovement>();
        ShielderAttack shielderAttack = GetComponentInParent<ShielderAttack>();
        
        // stop player tackle
        if (playerMovement.CurrentState is PlayerState.ShortAttackState)
        {
            PlayerState.ShortAttackState shortAttackState = (PlayerState.ShortAttackState)playerMovement.CurrentState;

            shortAttackState.IsPrevented = true;
        }

        // knockback
        if (enemyMovement != null && shielderAttack != null)
        {
            shielderAttack.KnockbackDirection = knockBackDirection;
            enemyMovement.CurrentState.SwitchState(enemyMovement.gameObject, ref enemyMovement.CurrentState, new KnockbackState());
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        IParriable parriable;
        if (collision.gameObject.TryGetComponent<IParriable>(out parriable))
        {
            parriable.IsParried = true;
        }
    }
}
