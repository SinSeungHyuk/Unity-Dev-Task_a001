using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class State<StateOwner>
{
    // 이 상태의 스테이트머신과 스테이트머신의 소유자(몬스터)
    public StateMachine<StateOwner> StateMachine { get; private set; }
    public StateOwner StateMachineOwner { get; private set; }

    // 스테이트머신에서 사용할 함수
    public void SetUp(StateMachine<StateOwner> owner, StateOwner type)
    {
        StateMachine = owner;
        StateMachineOwner = type;

        Awake();
    }

    // 각 스테이트의 변수 초기화 작업을 진행할 Awake 역할용
    protected virtual void Awake() { }

    // State가 시작될때
    public virtual void Enter() { }
    // State 실행중일때 프레임마다
    public virtual void Update() { }
    // State 나갈때
    public virtual void Exit() { }
}
