using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (_playerMovement != null)
            {

            }
        }

        public override void ExitState(GameObject gameObject)
        {
            throw new System.NotImplementedException();
        }


    }

    public class ShortAttackState : StateMachine
    {
        public override void EnterState(GameObject gameObject)
        {
            throw new System.NotImplementedException();
        }

        public override void ExitState(GameObject gameObject)
        {
            throw new System.NotImplementedException();
        }

        public override void UpdateState(GameObject gameObject)
        {
            throw new System.NotImplementedException();
        }
    }
}
