using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// made by KimDaehui
public abstract class StateMachine
{
    public abstract void EnterState(GameObject gameObject);
    public abstract void FixedUpdateState(GameObject gameObject);
    public abstract void UpdateState(GameObject gameObject);
    public abstract void ExitState(GameObject gameObject);
    public void SwitchState(GameObject gameObject, ref StateMachine currentState, StateMachine nextState)
    {
        if (gameObject == null || currentState == null || nextState == null) return;

        currentState.ExitState(gameObject);
        currentState = nextState;
        currentState.EnterState(gameObject);
    }
}
