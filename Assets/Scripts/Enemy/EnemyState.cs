using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// made by KimDaehui
public class PatrolState : EnemyStateMachine
{
    EnemyPatrol _enemyPatrol;
    float _elapsedTime;
    
    public override void EnterState(EnemyMovement enemyMovement)
    {
        _enemyPatrol = enemyMovement.GetComponent<EnemyPatrol>();
        _elapsedTime = _enemyPatrol.GetWaitingTime();
    }
    public override void UpdateState(EnemyMovement enemyMovement)
    {
        if (_enemyPatrol == null) return;

        if (_enemyPatrol.IsFindPlayer)
        {
            ChaseState chaseState = new ChaseState();
            SwitchState(enemyMovement, chaseState);
        }


        if (Vector2.Distance(enemyMovement.transform.position, _enemyPatrol.GetCurrentDestination()) < 0.5f)
        {
            _elapsedTime -= Time.deltaTime;

            if(_elapsedTime < 0f)
            {
                _enemyPatrol.SetDestination();
                _elapsedTime = _enemyPatrol.GetWaitingTime();
            }
        }
    }
    public override void ExitState(EnemyMovement enemyMovement)
    {
    }
}

public class ChaseState : EnemyStateMachine
{
    EnemyAttack _enemyAttack;

    public override void EnterState(EnemyMovement enemyMovement)
    {
        if(enemyMovement == null) return;
        // start move if no player in attack range
        if (enemyMovement.TryGetComponent<EnemyAttack>(out _enemyAttack) && !_enemyAttack.IsInAttackRange())
        {
            enemyMovement.StartMove();
        }
    }

    public override void UpdateState(EnemyMovement enemyMovement)
    {
        if (enemyMovement == null) return;

        // switch to ready when find player
        if (enemyMovement.TryGetComponent<EnemyAttack>(out _enemyAttack) && _enemyAttack.IsInAttackRange())
        {
            enemyMovement.StopMove();

            ReadyState readyState = new ReadyState();
            SwitchState(enemyMovement, readyState);
        }

        // chase player
        enemyMovement.Chase();
    }

    public override void ExitState(EnemyMovement enemyMovement)
    {
    }
}

public class ReadyState : EnemyStateMachine
{
    EnemyAttack _enemyAttack;
    public override void EnterState(EnemyMovement enemyMovement)
    {
        if (enemyMovement == null) return;

        // play ready sound
        if (enemyMovement.TryGetComponent<EnemyAttack>(out _enemyAttack) && _enemyAttack._readySFX != null)
        {
            enemyMovement.CalcSound_Direction_Distance();
            enemyMovement._audioSource.PlayOneShot(_enemyAttack._readySFX);
        }
        if (enemyMovement == null) return;
    }

    public override void UpdateState(EnemyMovement enemyMovement)
    {
        if (enemyMovement == null) return;
    }

    public override void ExitState(EnemyMovement enemyMovement)
    {
    }
}

public class AttackState : EnemyStateMachine
{
    EnemyAttack _enemyAttack;
    float elapsedTime;
    public override void EnterState(EnemyMovement enemyMovement)
    {
        if (enemyMovement == null) return;

        if(enemyMovement.TryGetComponent<EnemyAttack>(out _enemyAttack))
        {
            elapsedTime = _enemyAttack.GetAttackDelay();

            // attack differently by enemy's type
            if (_enemyAttack is LongRangeEnemyAttack)
            {
                LongRangeEnemyAttack enemyAttack_LongRange = (LongRangeEnemyAttack)_enemyAttack;
                enemyAttack_LongRange.Fire();
            }
            else
            {
                _enemyAttack._weapon.SetActive(true);

                EnemyShortWeapon shortWeapon;
                if (_enemyAttack._weapon.TryGetComponent<EnemyShortWeapon>(out shortWeapon))
                {
                    shortWeapon.StartAttack();
                }
            }

            // player attack sound
            if (_enemyAttack._attackSFX != null)
            {
                enemyMovement.CalcSound_Direction_Distance();
                enemyMovement._audioSource.PlayOneShot(_enemyAttack._attackSFX);
            }
        }
    }

    public override void UpdateState(EnemyMovement enemyMovement)
    {
        if (enemyMovement == null) return;

        // wait until attack ends
        elapsedTime -= Time.deltaTime;

        if (elapsedTime < 0f)
        {
            ChaseState chaseState = new ChaseState();
            SwitchState(enemyMovement, chaseState);
        }
    }

    public override void ExitState(EnemyMovement enemyMovement)
    {
        // attack ends
        if (_enemyAttack is not LongRangeEnemyAttack)
        {
            EnemyShortWeapon shortWeapon = _enemyAttack.GetComponentInChildren<EnemyShortWeapon>();
            if (shortWeapon != null)
            {
                shortWeapon.EndAttack();
            }
        }
    }
}
