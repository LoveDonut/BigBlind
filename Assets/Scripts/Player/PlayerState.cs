using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

// made by KimDaehui

namespace PlayerState
{
    // use Idlestate as MoveState temporarily because there is no move animation
    public class IdleState : StateMachine
    {
        PlayerMovement _playerMovement;
        PlayerRotate _playerRotate;

        public override void EnterState(GameObject gameObject)
        {
            _playerMovement = gameObject.GetComponent<PlayerMovement>();
            _playerRotate = gameObject.GetComponent<PlayerRotate>();
        }
        public override void UpdateState(GameObject gameObject)
        {
            if (_playerMovement == null || _playerRotate == null) return;

            _playerMovement.Move();
            _playerRotate.Rotate();
        }

        public override void ExitState(GameObject gameObject)
        {
        }
    }

    public class ShortAttackState : StateMachine
    {
        public bool IsBufferTime;
        public bool IsClickedOnBuffer;

        PlayerShortAttack _shortAttack;
        Animator _animator;
        Rigidbody2D _rigidbody;
        Vector2 _tackleDirection;
        Vector2 _startPosition;
        float _elapsedTime;
        float _elapsedDelay;

        public override void EnterState(GameObject gameObject)
        {
            _shortAttack = gameObject.GetComponent<PlayerShortAttack>();
            _animator = gameObject.GetComponent<Animator>();
            _rigidbody = gameObject.GetComponent<Rigidbody2D>();

            IsBufferTime = false;
            IsClickedOnBuffer = false;

            if(_animator != null)
            {
                _animator.SetTrigger("DoShortAttack");
            }

            if(_shortAttack != null)
            {
                _shortAttack.CanAttack = false;
                _elapsedTime = _shortAttack.ShortAttackAnimationClip.length;
                _elapsedDelay = _shortAttack._delayAfterTackle;
            }

            _startPosition = gameObject.transform.position;
            _tackleDirection = (Camera.main.ScreenToWorldPoint(Input.mousePosition) - gameObject.transform.position).normalized;

            //Rigidbody2D rigidbody;
            //if(gameObject.TryGetComponent<Rigidbody2D>(out rigidbody))
            //{
            //    _shortAttack.TackleVelocity = rigidbody.velocity * _tackleDirection;
            //}
        }
        public override void UpdateState(GameObject gameObject)
        {
            if (_shortAttack == null) return;

            _shortAttack.CollideWithEnemy();
            _elapsedTime -= Time.fixedDeltaTime;

            if (Vector2.Distance(_startPosition, gameObject.transform.position) < _shortAttack.ShortAttackDistance &&
                _elapsedTime > 0)
            {
                _shortAttack.Tackle(_tackleDirection);
            }
            else if(_elapsedDelay > 0)
            {
                if (_rigidbody != null)
                {
                    _rigidbody.velocity = Vector2.zero;
                }

                PlayerShoot playerShoot;
                if(gameObject.TryGetComponent<PlayerShoot>(out playerShoot))
                {
                    if (_elapsedDelay < playerShoot.BufferDuration)
                    {
                        IsBufferTime = true;
                    }
                }

                _elapsedDelay -= Time.fixedDeltaTime;
            }
            else
            {
                PlayerMovement playerMovement;
                if (gameObject.TryGetComponent<PlayerMovement>(out playerMovement))
                {
                    SwitchState(gameObject, ref playerMovement.CurrentState, new IdleState());
                }
            }
        }

        public override void ExitState(GameObject gameObject)
        {
            if (_rigidbody != null)
            {
                _rigidbody.velocity = Vector2.zero;
            }

            PlayerShoot playerShoot;
            PlayerShortAttack playerShortAttack;
            if (gameObject.TryGetComponent<PlayerShoot>(out playerShoot) && gameObject.TryGetComponent<PlayerShortAttack>(out playerShortAttack))
            {
                // do not shoot if player will tackle instantly
                if (IsClickedOnBuffer && !playerShortAttack.IsClickedOnBuffer)
                {
                    playerShoot.Shoot();
                }
            }
        }

    }
}
