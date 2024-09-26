using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : StateMachine
{
    public override void EnterState(Enemy enemy)
    {
        if (enemy == null) return;

        if (!enemy.IsInAttackRange())
        {
            enemy.StartMove();
        }
    }

    public override void UpdateState(Enemy enemy)
    {
        if (enemy == null) return;

        if (enemy.IsInAttackRange())
        {
            enemy.StopMove();

            ReadyState readyState = new ReadyState();
            SwitchState(enemy, readyState);
        }
        enemy.Chase();
    }
}

public class ReadyState : StateMachine
{
    public override void EnterState(Enemy enemy)
    {
        if (enemy._readySFX != null)
        {
            enemy._audioSource.PlayOneShot(enemy._readySFX);
        }
        if (enemy == null) return;
    }

    public override void UpdateState(Enemy enemy)
    {
        if (enemy == null) return;
    }
}

public class AttackState : StateMachine
{
    float elapsedTime;
    public override void EnterState(Enemy enemy)
    {
        if (enemy == null) return;

        elapsedTime = enemy.GetAttackDelay();

        if (enemy is Enemy_LongRange)
        {
            Enemy_LongRange enemy_LongRange = (Enemy_LongRange)enemy;
            enemy_LongRange.Fire();
        }

        if (enemy._AttackSFX != null)
        {
            enemy._audioSource.PlayOneShot(enemy._AttackSFX);
        }
    }

    public override void UpdateState(Enemy enemy)
    {
        if (enemy == null) return;

        elapsedTime -= Time.deltaTime;

        if (elapsedTime < 0f)
        {
            ChaseState chaseState = new ChaseState();
            SwitchState(enemy, chaseState);
        }
    }
}
