using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// made by KimDaehui
public abstract class StateMachine
{
    public abstract void EnterState(Enemy enemy);
    public abstract void UpdateState(Enemy enemy);
    public abstract void ExitState(Enemy enemy);
    public void SwitchState(Enemy enemy, StateMachine enemyState)
    {
        if (enemy == null || enemyState == null) return;

        enemy._currentState.ExitState(enemy);
        enemy._currentState = enemyState;
        enemy._currentState.EnterState(enemy);
    }
}