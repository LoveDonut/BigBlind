using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseState : StateMachine
{
    public override void EnterState(Enemy enemy)
    {
        if (enemy == null) return;

        // start move if no player in attack range
        if (!enemy.IsInAttackRange())
        {
            enemy.StartMove();
        }
    }

    public override void UpdateState(Enemy enemy)
    {
        if (enemy == null) return;

        // switch to ready when find player
        if (enemy.IsInAttackRange())
        {
            enemy.StopMove();

            ReadyState readyState = new ReadyState();
            SwitchState(enemy, readyState);
        }

        // chase player
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
        // play ready sound
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

        // attack differently by enemy's type
        if (enemy is Enemy_LongRange)
        {
            Enemy_LongRange enemy_LongRange = (Enemy_LongRange)enemy;
            enemy_LongRange.Fire();
        }
        else
        {
            enemy._weapon.SetActive(true);

            ShortWeapon_Enemy shortWeapon;
            if (enemy._weapon.TryGetComponent<ShortWeapon_Enemy>(out shortWeapon))
            {
                shortWeapon.StartAttack();
            }
        }

        // player attack sound
        if (enemy._AttackSFX != null)
        {
            enemy._audioSource.PlayOneShot(enemy._AttackSFX);
        }
    }

    public override void UpdateState(Enemy enemy)
    {
        if (enemy == null) return;

        // wait until attack ends
        elapsedTime -= Time.deltaTime;

        if (elapsedTime < 0f)
        {
            ChaseState chaseState = new ChaseState();
            SwitchState(enemy, chaseState);
        }
    }

    public override void ExitState(Enemy enemy)
    {
        // attack ends
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
