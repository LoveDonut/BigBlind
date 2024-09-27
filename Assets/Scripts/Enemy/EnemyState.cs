using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// made by KimDaehui
public class PatrolState : StateMachine
{
    EnemyPatrol _enemyPatrol;
    float _elapsedTime;
    
    public override void EnterState(Enemy enemy)
    {
        _enemyPatrol = enemy.GetComponent<EnemyPatrol>();
        _elapsedTime = _enemyPatrol.GetWaitingTime();
    }
    public override void UpdateState(Enemy enemy)
    {
        if (_enemyPatrol == null) return;

        if (_enemyPatrol.IsFindPlayer)
        {
            ChaseState chaseState = new ChaseState();
            SwitchState(enemy, chaseState);
        }


        if (Vector2.Distance(enemy.transform.position, _enemyPatrol.GetCurrentDestination()) < 0.5f)
        {
            _elapsedTime -= Time.deltaTime;

            if(_elapsedTime < 0f)
            {
                _enemyPatrol.SetDestination();
                _elapsedTime = _enemyPatrol.GetWaitingTime();
            }
        }
    }
    public override void ExitState(Enemy enemy)
    {
    }
}

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
            enemy.CalcSound_Direction_Distance();
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
        if (enemy is LongRangeEnemy)
        {
            LongRangeEnemy enemy_LongRange = (LongRangeEnemy)enemy;
            enemy_LongRange.Fire();
        }
        else
        {
            enemy._weapon.SetActive(true);

            EnemyShortWeapon shortWeapon;
            if (enemy._weapon.TryGetComponent<EnemyShortWeapon>(out shortWeapon))
            {
                shortWeapon.StartAttack();
            }
        }

        // player attack sound
        if (enemy._AttackSFX != null)
        {
            enemy.CalcSound_Direction_Distance();
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
        if (enemy is not LongRangeEnemy)
        {
            EnemyShortWeapon shortWeapon = enemy.GetComponentInChildren<EnemyShortWeapon>();
            if (shortWeapon != null)
            {
                shortWeapon.EndAttack();
            }
        }
    }
}
