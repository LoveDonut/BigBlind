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

    public override void ExitState(Enemy enemy)
    {
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

    public override void ExitState(Enemy enemy)
    {
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
        else
        {
            ShortWeapon_Enemy shortWeapon = enemy.GetComponentInChildren<ShortWeapon_Enemy>();
            if (shortWeapon != null)
            {
                shortWeapon.StartAttack();
            }
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

    public override void ExitState(Enemy enemy)
    {
        if (enemy is not Enemy_LongRange)
        {
            ShortWeapon_Enemy shortWeapon = enemy.GetComponentInChildren<ShortWeapon_Enemy>();
            if (shortWeapon != null)
            {
                shortWeapon.EndAttack();
            }
        }
    }
}
