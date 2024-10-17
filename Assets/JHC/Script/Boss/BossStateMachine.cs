using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine : MonoBehaviour
{
    private BossState currentState;

    protected void Awake()
    {
    }

    // STATE Change
    public void ChangeState(BossState newState)
    {
        // end currentState
        if (currentState != null)
        {
            currentState.Exit();
        }

        // new State enter
        currentState = newState;
        currentState.Enter(this);
    }

    void FixedUpdate()
    {
        // call update in currrentState
        if (currentState != null)
        {
            currentState.Update();
        }
    }

    // change state after delayTime
    public void DelayChangeState(float time, BossState state)
    {
        StartCoroutine(StateChangeDelayRoutine(time, state));
    }

    IEnumerator StateChangeDelayRoutine(float time, BossState state)
    {
        yield return new WaitForSeconds(time);
        ChangeState(state);
    }
}

/*===================================================================*/

class BossIdleState : BossState
{

    public override void Enter(BossStateMachine boss)
    {
        base.Enter(boss);
    }



    public override void Update()
    {

    }

    public override void Exit()
    {

    }
}
