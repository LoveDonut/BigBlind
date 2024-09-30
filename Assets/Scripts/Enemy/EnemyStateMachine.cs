using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// made by KimDaehui
public abstract class EnemyStateMachine
{
    public abstract void EnterState(EnemyMovement enemyMovement);
    public abstract void UpdateState(EnemyMovement enemyMovement);
    public abstract void ExitState(EnemyMovement enemyMovement);
    public void SwitchState(EnemyMovement enemyMovement, EnemyStateMachine enemyState)
    {
        if (enemyMovement == null || enemyState == null) return;

        enemyMovement._currentState.ExitState(enemyMovement);
        enemyMovement._currentState = enemyState;
        enemyMovement._currentState.EnterState(enemyMovement);
    }
}
