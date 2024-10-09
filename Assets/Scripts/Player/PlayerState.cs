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
        PlayerShortAttack _shortAttack;
        Animator _animator;
        Vector2 _tackleDirection;
        Vector2 _startPosition;
        float _elapsedTime;

        public override void EnterState(GameObject gameObject)
        {
            _shortAttack = gameObject.GetComponent<PlayerShortAttack>();
            _animator = gameObject.GetComponent<Animator>();
            if(_animator != null)
            {
                _animator.SetTrigger("DoShortAttack");
            }

            _shortAttack.CanAttack = false;
            _elapsedTime = _shortAttack.ShortAttackAnimationClip.length;

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
            Rigidbody2D rigidbody;
            if(gameObject.TryGetComponent<Rigidbody2D>(out rigidbody))
            {
                rigidbody.velocity = Vector2.zero;
            }
        }

    }
}
