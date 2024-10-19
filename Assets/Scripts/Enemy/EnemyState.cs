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
        public override void FixedUpdateState(GameObject enemy)
        {
        }
        public override void ExitState(GameObject enemy)
        {
            EnemyAttack enemyAttack;

            if (enemy.TryGetComponent<EnemyAttack>(out enemyAttack))
            {
                enemyAttack.EndSleep();
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
        WaveManager _waveManager;
        NavMeshAgent _navMeshAgent;
        float _elapsedTime;
        float _originalSpeed;

        public override void EnterState(GameObject enemy)
        {
            _enemyPatrol = enemy.GetComponent<EnemyPatrol>();
            _enemyMovement = enemy.GetComponent<EnemyMovement>();
            _elapsedTime = _enemyPatrol.GetWaitingTime();
            _waveManager = enemy.GetComponent<WaveManager>();
            _navMeshAgent = enemy.GetComponent<NavMeshAgent>();

            if (_waveManager != null && _enemyPatrol != null && _navMeshAgent != null)
            {
                _waveManager.BPM *= _enemyPatrol.PatrolBpmMultiplier;
                _originalSpeed = _navMeshAgent.speed;
                _navMeshAgent.speed = _enemyPatrol.PatrolMoveSpeed;
            }
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
        public override void FixedUpdateState(GameObject enemy)
        {
        }
        public override void ExitState(GameObject enemy)
        {
            if (_waveManager != null && _enemyPatrol != null && _navMeshAgent != null)
            {
                _waveManager.BPM /= _enemyPatrol.PatrolBpmMultiplier;
                _navMeshAgent.speed = _originalSpeed;
            }
        }
    }
    public class ChaseState : StateMachine
    {
        EnemyAttack _enemyAttack;
        EnemyMovement _enemyMovement;

        public override void EnterState(GameObject enemy)
        {
            _enemyAttack = enemy.GetComponent<EnemyAttack>();
            _enemyMovement = enemy.GetComponent<EnemyMovement>();

            // start move if no player in attack range
            if (_enemyAttack != null)
            {
                _enemyAttack.InitChase();
            }

        }

        public override void UpdateState(GameObject enemy)
        {
            // switch to ready when find player
            if (_enemyAttack != null && _enemyAttack.IsInAttackRange())
            {
                if(_enemyMovement != null)
                {
                    _enemyMovement.StopMove();
                }

                ReadyState readyState = new ReadyState();
                SwitchState(enemy, ref _enemyMovement.CurrentState, readyState);
            }
            // chase player
            if(_enemyMovement !=null)
            {
                _enemyMovement.Chase();
            }
        }
        public override void FixedUpdateState(GameObject enemy)
        {
        }

        public override void ExitState(GameObject enemy)
        {
            if (_enemyAttack != null)
            {
                _enemyAttack.EndChase();
            }
        }
    }
    public class ReadyState : StateMachine
    {
        EnemyAttack _enemyAttack;
        public override void EnterState(GameObject enemy)
        {
            _enemyAttack = enemy.GetComponent<EnemyAttack>();

            if (_enemyAttack == null) return;

            // change BPM
            WaveManager waveManager;
            if (enemy.TryGetComponent<WaveManager>(out waveManager))
            {
                waveManager.BPM *= _enemyAttack._bpmMultiplier;
            }
            // play ready sound
            if (_enemyAttack._readySFX != null)
            {
                SoundManager.Instance.PlaySound(_enemyAttack._readySFX, _enemyAttack.transform.position);
            }
        }

        public override void UpdateState(GameObject enemy)
        {
        }
        public override void FixedUpdateState(GameObject enemy)
        {
        }

        public override void ExitState(GameObject enemy)
        {            
            // recover BPM
            WaveManager waveManager;
            if (enemy.TryGetComponent<WaveManager>(out waveManager))
            {
                waveManager.BPM /= _enemyAttack._bpmMultiplier;
            }
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
            _enemyAttack = enemy.GetComponent<EnemyAttack>();
            if (_enemyAttack == null) return;

            elapsedTime = _enemyAttack.GetAttackDelay();

            // attack differently by enemy's type
            _enemyAttack.InitAttack();

            // player attack sound
            if (_enemyAttack._attackSFX != null)
            {
                SoundManager.Instance.PlaySound(_enemyAttack._attackSFX, _enemyAttack.transform.position);
            }
        }

        public override void UpdateState(GameObject enemy)
        {
            if (enemy == null) return;

            _enemyAttack.UpdateAttack();

            // wait until attack ends
            elapsedTime -= Time.deltaTime;

            if (elapsedTime < 0f)
            {
                ChaseState chaseState = new ChaseState();
                SwitchState(enemy, ref _enemyMovement.CurrentState, chaseState);
            }
        }
        public override void FixedUpdateState(GameObject enemy)
        {
            _enemyAttack.FixedUpdateAttack();
        }

        public override void ExitState(GameObject enemy)
        {
            // attack ends
            _enemyAttack.EndAttack();

            //// recover BPM
            //WaveManager waveManager;
            //if (enemy.TryGetComponent(out waveManager))
            //{
            //    waveManager.BPM /= _enemyAttack._bpmMultiplier;
            //    Debug.Log("BPM DOWN!");
            //}
        }
    }
    public class KnockbackState : StateMachine
    {
        EnemyMovement _enemyMovement;
        ShielderAttack _shielderAttack;
        Rigidbody2D _rigidbody;

        Vector2 _startPosition;
        Vector2 velocity;
        float elapsedTime;
        float knockbackDuration = 1f;
        public override void EnterState(GameObject enemy)
        {
            _enemyMovement = enemy.GetComponent<EnemyMovement>();
            _shielderAttack = enemy.GetComponent<ShielderAttack>();
            _rigidbody = enemy.GetComponent<Rigidbody2D>();

            elapsedTime = knockbackDuration;
            _startPosition = enemy.transform.position;
            velocity = _shielderAttack.KnockbackSpeed * _shielderAttack.KnockbackDirection;
        }

        public override void UpdateState(GameObject enemy)
        {
        }
        public override void FixedUpdateState(GameObject enemy)
        {
            if(Vector2.Distance(_startPosition, enemy.transform.position) < _shielderAttack.KnockbackDistance && elapsedTime > 0f)
            {
                if (_shielderAttack != null && _rigidbody != null)
                {
                    velocity = Vector2.MoveTowards(velocity, Vector2.zero, _shielderAttack.KnockbackDecceleration * Time.fixedDeltaTime);
//                    _rigidbody.velocity = _shielderAttack.KnockbackSpeed * _shielderAttack.KnockbackDirection;
                    _rigidbody.velocity = velocity;
                }
            }
            else
            {
                if(_enemyMovement != null)
                {
                    SwitchState(_enemyMovement.gameObject, ref _enemyMovement.CurrentState, new ChaseState());
                }
            }

            elapsedTime -= Time.fixedDeltaTime;
        }

        public override void ExitState(GameObject enemy)
        {
            _rigidbody.velocity = Vector2.zero;
            // recover BPM
            WaveManager waveManager;
            if (enemy.TryGetComponent<WaveManager>(out waveManager))
            {
                waveManager.BPM = SoundManager.Instance.BPM * waveManager._bpmMultiplier;
            }
        }
    }
    public class StunState : StateMachine
    {
        public float ElapsedTime;
        EnemyMovement _enemyMovement;
        public override void EnterState(GameObject gameObject)
        {
            _enemyMovement = gameObject.GetComponent<EnemyMovement>();
            if (_enemyMovement != null)
            {
                _enemyMovement.StopMove();
            }
        }
        public override void FixedUpdateState(GameObject gameObject)
        {
        }
        public override void UpdateState(GameObject gameObject)
        {
            if(ElapsedTime < 0)
            {
                SetActiveState();
            }
            ElapsedTime -= Time.deltaTime;
        }
        public override void ExitState(GameObject gameObject)
        {
            if (_enemyMovement != null)
            {
                _enemyMovement.StartMove();
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
}

