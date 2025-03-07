using System;
using System.Collections.Generic;
using UnityEngine;

// 몬스터가 상속받을 스테이트머신
public abstract class MonsterStateMachine<Owner> : MonoBehaviour
{
    /// <summary>
    /// 추상클래스로 스테이트머신을 하나 더 만드는 이유
    /// -> 일반클래스인 stateMachine 객체를 생성해서 stateMachine의 함수를 호출해야 하는데
    /// 매번 stateMachine를 붙여서 함수를 호출하기에 번거롭고 가독성이 떨어지기 때문
    /// 즉, 이 추상클래스는 Wrapping을 위해 존재 (+ Update,SetUp 등 코드의 중복 줄이기)
    /// </summary>


    // 스테이트 변경될때 호출될 이벤트
    // <스테이트머신, 새 스테이트, 이전 스테이트, 레이어>
    public event Action<StateMachine<Owner>, State<Owner>, State<Owner>> OnStateChanged;

    private readonly StateMachine<Owner> stateMachine = new();

    public Owner StateMachineOwner => stateMachine.StateMachineOwner;


    private void Update()
    {
        // StateMachine 클래스는 MonoBehaviour가 없어서 업데이트가 호출이 안됨
        // EntityStateMachine를 상속받아 만들어질 진짜 스테이트머신의 업데이트에서 업데이트
        if (this.StateMachineOwner != null)
            stateMachine.Update();
    }

    public void InitializeStateMachine(Owner owner)
    {
        // 1. 스테이머신 초기화 -> owner(monster)를 넣어서 스테이트머신의 주인을 알려줌
        // 2. 스테이트와 트랜지션들 추가 -> 상속받은 자식클래스에서 구현한대로 상태,트랜지션을 추가해줌
        // 3. 상태와 트랜지션은 추가되면서 스테이트머신이 가지고 있는 주인을 바탕으로 생성됨
        // 4. 맨처음 시작되어야하는 기본상태 시작

        stateMachine.InitializeStateMachine(owner);
        AddStates();
        MakeTransitions();
        stateMachine.SetUpDefaultState();

        stateMachine.OnStateChanged += (_, newState, prevState)
            => OnStateChanged?.Invoke(stateMachine, newState, prevState);
    }

    #region StateMachine Wrapping
    public void AddState<T>()
        where T : State<Owner>
        => stateMachine.AddState<T>();

    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand,
        Func<State<Owner>, bool> transitionCondition)
        where FromStateType : State<Owner>
        where ToStateType : State<Owner>
        => stateMachine.MakeTransition<FromStateType, ToStateType>(transitionCommand, transitionCondition);

    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand,
        Func<State<Owner>, bool> transitionCondition)
        where FromStateType : State<Owner>
        where ToStateType : State<Owner>
        => stateMachine.MakeTransition<FromStateType, ToStateType>(transitionCommand, transitionCondition);

    public void MakeTransition<FromStateType, ToStateType>(Func<State<Owner>, bool> transitionCondition)
        where FromStateType : State<Owner>
        where ToStateType : State<Owner>
        => stateMachine.MakeTransition<FromStateType, ToStateType>(int.MinValue, transitionCondition);

    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand)
        where FromStateType : State<Owner>
        where ToStateType : State<Owner>
        => stateMachine.MakeTransition<FromStateType, ToStateType>(transitionCommand, null);

    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand)
        where FromStateType : State<Owner>
        where ToStateType : State<Owner>
        => stateMachine.MakeTransition<FromStateType, ToStateType>(transitionCommand, null);


    public bool ExecuteCommand(int transitionCommand)
        => stateMachine.ExecuteCommand(transitionCommand);
    public bool ExecuteCommand(Enum transitionCommand)
        => stateMachine.ExecuteCommand(transitionCommand);


    public State<Owner> GetCurrentState() => stateMachine.GetCurrentState();

    public Type GetCurrentStateType() => stateMachine.GetCurrentStateType();
    #endregion


    // 이 스테이트머신을 상속받아 사용하게 될 '진짜 스테이트머신'에서 구현
    protected abstract void AddStates();
    protected abstract void MakeTransitions();
}
