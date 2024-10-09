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

        public override void EnterState(GameObject gameObject)
        {
            _playerMovement = gameObject.GetComponent<PlayerMovement>();
        }
        public override void UpdateState(GameObject gameObject)
        {
            if (_playerMovement == null) return;

            _playerMovement.Move();
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
        float _tackleElapsedTime;

        public override void EnterState(GameObject gameObject)
        {
            _shortAttack = gameObject.GetComponent<PlayerShortAttack>();
            if(gameObject.TryGetComponent<Animator>(out  _animator))
            {
                _animator.SetTrigger("DoShortAttack");
            }

            _shortAttack.CanAttack = false;
            _tackleElapsedTime = _shortAttack.ShortAttackDuration;

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

            _shortAttack.AttackCollideWithEnemy();

            if (_tackleElapsedTime > 0)
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

            _tackleElapsedTime -= Time.fixedDeltaTime;
        }

        public override void ExitState(GameObject gameObject)
        {
            _tackleElapsedTime = _shortAttack.ShortAttackDuration;
            
            Rigidbody2D rigidbody;
            if(gameObject.TryGetComponent<Rigidbody2D>(out rigidbody))
            {
                rigidbody.velocity = Vector2.zero;
            }
        }

    }
}
