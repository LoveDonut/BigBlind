using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// made by KimDaehui

namespace EnemyState
{
    public class SleepState : StateMachine
    {
        EnemyMovement _enemyMovement;
        public override void EnterState(GameObject enemy)
        {
            _enemyMovement = enemy.GetComponent<EnemyMovement>();
        }
        public override void UpdateState(GameObject enemy)
        {
            if (_enemyMovement.IsActive)
            {
                SetActiveState();
            }
        }
        public override void ExitState(GameObject enemy)
        {
            InitWave(enemy);
        }

        private static void InitWave(GameObject enemy)
        {
            WaveManager waveManager;

            if (enemy.TryGetComponent<WaveManager>(out waveManager))
            {
                waveManager.EnqueueWaveForPlayingByBeat();
            }

            EnemyAttack enemyAttack;

            if (enemy.TryGetComponent<EnemyAttack>(out enemyAttack))
            {
                enemyAttack.ResetReadyBeatCount();
            }
        }

        void SetActiveState()
        {
            EnemyPatrol enemyPatrol;

            if (_enemyMovement.TryGetComponent<EnemyPatrol>(out enemyPatrol))
            {
                _enemyMovement.CurrentState.SwitchState(_enemyMovement.gameObject, ref _enemyMovement.CurrentState, new PatrolState());
            }
            else
            {
                _enemyMovement.CurrentState.SwitchState(_enemyMovement.gameObject, ref _enemyMovement.CurrentState, new ChaseState());
            }
        }
    }
    public class PatrolState : StateMachine
    {
        EnemyPatrol _enemyPatrol;
        EnemyMovement _enemyMovement;
        float _elapsedTime;

        public override void EnterState(GameObject enemy)
        {
            _enemyPatrol = enemy.GetComponent<EnemyPatrol>();
            _enemyMovement = enemy.GetComponent<EnemyMovement>();
            _elapsedTime = _enemyPatrol.GetWaitingTime();
        }
        public override void UpdateState(GameObject enemy)
        {
            if (_enemyPatrol == null || _enemyMovement == null) return;

            if (_enemyPatrol.IsFindPlayer)
            {
                ChaseState chaseState = new ChaseState();
                SwitchState(enemy, ref _enemyMovement.CurrentState, chaseState);
            }


            if (Vector2.Distance(enemy.transform.position, _enemyPatrol.GetCurrentDestination()) < 0.5f)
            {
                _elapsedTime -= Time.deltaTime;

                if (_elapsedTime < 0f)
                {
                    _enemyPatrol.SetDestination();
                    _elapsedTime = _enemyPatrol.GetWaitingTime();
                }
            }
        }
        public override void ExitState(GameObject enemy)
        {
        }
    }
    public class ChaseState : StateMachine
    {
        EnemyAttack _enemyAttack;
        EnemyMovement _enemyMovement;

        public override void EnterState(GameObject enemy)
        {
            _enemyMovement = enemy.GetComponent<EnemyMovement>();

            if(_enemyMovement == null) return;

            // start move if no player in attack range
            if (enemy.TryGetComponent<EnemyAttack>(out _enemyAttack) && !_enemyAttack.IsInAttackRange())
            {
                _enemyMovement.StartMove();
            }
        }

        public override void UpdateState(GameObject enemy)
        {
            if (_enemyMovement == null) return;

            // switch to ready when find player
            if (enemy.TryGetComponent<EnemyAttack>(out _enemyAttack) && _enemyAttack.IsInAttackRange())
            {
                _enemyMovement.StopMove();

                ReadyState readyState = new ReadyState();
                SwitchState(enemy, ref _enemyMovement.CurrentState, readyState);
            }

            // chase player
            _enemyMovement.Chase();
        }

        public override void ExitState(GameObject enemy)
        {
        }
    }

    public class ReadyState : StateMachine
    {
        EnemyAttack _enemyAttack;
        EnemyMovement _enemyMovement;
        public override void EnterState(GameObject enemy)
        {
            _enemyMovement = enemy.GetComponent<EnemyMovement>();

            if (_enemyMovement == null) return;

            // play ready sound
            if (enemy.TryGetComponent<EnemyAttack>(out _enemyAttack) && _enemyAttack._readySFX != null)
            {
                // change BPM
                WaveManager waveManager;
                if (enemy.TryGetComponent(out waveManager))
                {
                    waveManager.BPM *= _enemyAttack._bpmMultiplier;
//                    Debug.Log("BPM UP!");
                }
                SoundManager.Instance.PlaySound(_enemyAttack._readySFX, _enemyAttack.transform.position);
            }
        }

        public override void UpdateState(GameObject enemy)
        {
        }

        public override void ExitState(GameObject enemy)
        {
        }
    }

    public class AttackState : StateMachine
    {
        EnemyAttack _enemyAttack;
        EnemyMovement _enemyMovement;
        float elapsedTime;
        public override void EnterState(GameObject enemy)
        {
            _enemyMovement = enemy.GetComponent<EnemyMovement>();
            if (_enemyMovement == null) return;

            if (enemy.TryGetComponent<EnemyAttack>(out _enemyAttack))
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
                    _enemyAttack.Weapon.SetActive(true);

                    EnemyShortWeapon shortWeapon;
                    if (_enemyAttack.Weapon.TryGetComponent<EnemyShortWeapon>(out shortWeapon))
                    {
                        shortWeapon.StartAttack();
                    }
                }

                // player attack sound
                if (_enemyAttack._attackSFX != null)
                {
                    SoundManager.Instance.PlaySound(_enemyAttack._attackSFX, _enemyAttack.transform.position);
                }

                // recover BPM
                WaveManager waveManager;
                if (enemy.TryGetComponent(out waveManager))
                {
                    waveManager.BPM /= _enemyAttack._bpmMultiplier;
//                    Debug.Log("BPM DOWN!");
                }
            }
        }

        public override void UpdateState(GameObject enemy)
        {
            if (enemy == null) return;

            // wait until attack ends
            elapsedTime -= Time.deltaTime;

            if (elapsedTime < 0f)
            {
                ChaseState chaseState = new ChaseState();
                SwitchState(enemy, ref _enemyMovement.CurrentState, chaseState);
            }
        }

        public override void ExitState(GameObject enemy)
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

            //// recover BPM
            //WaveManager waveManager;
            //if (enemy.TryGetComponent(out waveManager))
            //{
            //    waveManager.BPM /= _enemyAttack._bpmMultiplier;
            //    Debug.Log("BPM DOWN!");
            //}
        }
    }
}

