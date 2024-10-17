using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossState {

    protected BossStateMachine boss;
    protected float _beforeStateDelayTime;// 선딜 시간
    protected float _stateTime;           // 패턴 시간 
    protected float _afterStateDelayTime; // 후딜 시간

    // 상태가 시작될 때 호출
    public virtual void Enter(BossStateMachine boss)
    {
        Debug.Log(this.ToString());
        this.boss = boss; 
    }

    // 상태가 매 프레임마다 호출
    public virtual void Update()
    {
    }

    // 상태가 종료될 때 호출
    public virtual void Exit()
    {
    }
}