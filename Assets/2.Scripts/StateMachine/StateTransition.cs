using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class StateTransition<StateOwner>
{
    // 트랜지션의 커맨드가 없을때
    public const int NullCommand = int.MinValue;

    // 전이를 위한 조건 함수
    // Func<T1,T2> : T1을 매개변수로 T2를 반환 (Func 함수의 결과 bool 반환)
    private Func<State<StateOwner>, bool> transitionCondition;

    // 현재 스테이트
    public State<StateOwner> FromState { get; private set; }
    // FromState로부터 전이할 스테이트
    public State<StateOwner> ToState { get; private set; }
    // 전이 명령어 (enum으로 사용할 예정)
    public int TransitionCommand { get; private set; }
    // 전이 가능한지 여부 (전이함수가 없거나 FromState로 가는게 true일 경우)
    public bool IsTransferable => transitionCondition == null || transitionCondition.Invoke(FromState);


    // 트랜지션 생성자
    public StateTransition(State<StateOwner> fromState,
       State<StateOwner> toState,
       int transitionCommand,
       Func<State<StateOwner>, bool> transitionCondition)
    {
        FromState = fromState;
        ToState = toState;
        TransitionCommand = transitionCommand;
        this.transitionCondition = transitionCondition;
    }
}
