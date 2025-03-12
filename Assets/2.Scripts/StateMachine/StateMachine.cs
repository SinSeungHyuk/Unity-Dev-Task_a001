using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.UI.GridLayoutGroup;


public class StateMachine<StateOwner>
{
    public event Action
        <StateMachine<StateOwner>, State<StateOwner>, State<StateOwner>> OnStateChanged;

    // State의 정보를 담아서 관리할 이너클래스
    private class StateData
    {
        // 스테이트의 등록 순서 (등록한 순서대로 우선순위 부여)
        public int Priority { get; private set; }
        // 이 data가 가진 스테이트
        public State<StateOwner> State { get; private set; }
        // 이 스테이트와 연결된 트랜지션들
        public List<StateTransition<StateOwner>> Transitions { get; private set; } = new();

        // StateData 생성자
        public StateData(int priority, State<StateOwner> state)
        {
            Priority = priority;
            State = state;
        }
    }

    // <스테이트의 클래스 타입, 스테이트 데이터>
    private readonly Dictionary<Type, StateData> stateDatas = new();

    private StateData currentStateData;

    public StateOwner StateMachineOwner { get; private set; }


    // MonoBehaviour가 없기 때문에 실제 Update되진 않음
    // 이걸 상속받는 자식 스테이트머신의 Update로 수동 호출
    public void Update()
    {
        // 현재 실행중인 스테이트의 트랜지션 조건들을 검사
        TryTransition(currentStateData.Transitions);

        // 전이하지 못했다면 현재 State의 Update를 실행함
        currentStateData.State.Update();
    }

    public void InitializeStateMachine(StateOwner owner)
    {
        StateMachineOwner = owner;
    }

    public void SetUpDefaultState()
    {
        // 우선 순위가 가장 높은 StateData를 찾아옴
        StateData firstStateData = stateDatas.Values.FirstOrDefault(x => x.Priority == 0);
        // 찾아온 StateData의 State를 Current State로 설정해줌
        ChangeState(firstStateData);
    }

    // 현재 실행중인 CurrentStateData를 변경하는 함수
    private void ChangeState(StateData newStateData)
    {
        // 현재 실행중인 CurrentStateData를 가져옴
        StateData prevState = currentStateData;

        prevState?.State.Exit();
        // 현재 실행중인 CurrentStateData를 인자로 받은 newStateData로 교체해줌
        currentStateData = newStateData;
        newStateData.State.Enter();

        // State가 전이되었음을 알림
        OnStateChanged?.Invoke(this, newStateData.State, prevState?.State);
    }

    private void ChangeState(State<StateOwner> newState)
    {
        // StateDatas 중 newState를 가진 StateData를 찾아옴
        StateData newStateData = stateDatas[newState.GetType()];
        ChangeState(newStateData);
    }

    private bool TryTransition(IReadOnlyList<StateTransition<StateOwner>> transtions)
    {
        foreach (var transition in transtions)
        {
            // enum 커맨드가 null이 아니거나 트랜지션 Func가 false일 경우 전이 실패
            if (transition.TransitionCommand != StateTransition<StateOwner>.NullCommand || !transition.IsTransferable)
                continue;

            // 모든 조건을 만족한다면 ToState로 전이
            ChangeState(transition.ToState);
            return true;
        }
        return false;
    }


    public void AddState<T>() where T : State<StateOwner>
    {
        // Type을 통해 State를 생성
        State<StateOwner> newState = Activator.CreateInstance<T>();
        newState.SetUp(this, StateMachineOwner);

        // AddState를 호출하는 순서가 곧 스테이트의 우선순위
        stateDatas.Add(typeof(T), new StateData(stateDatas.Count, newState));
    }

    // FromStateType은 현재 State의 Type
    // ToStateType은 전이할 State의 Type
    // 두 Tpye 모두 State<Owner> class를 자식이여야함
    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand,
        Func<State<StateOwner>, bool> transitionCondition)
        where FromStateType : State<StateOwner>
        where ToStateType : State<StateOwner>
    {
        // StateDatas에서 FromStateType의 State를 가진 StateData를 찾아옴
        StateData fromStateData = stateDatas[typeof(FromStateType)];
        // StateDatas에서 ToStateType의 State를 가진 StateData를 찾아옴
        StateData toStateData = stateDatas[typeof(ToStateType)];

        var newTransition = new StateTransition<StateOwner>(fromStateData.State, toStateData.State,
            transitionCommand, transitionCondition);
        // 생성한 Transition을 FromStateData의 Transition으로 추가
        fromStateData.Transitions.Add(newTransition);
    }
    #region Make Transition Wrapping
    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand,
        Func<State<StateOwner>, bool> transitionCondition)
        where FromStateType : State<StateOwner>
        where ToStateType : State<StateOwner>
        => MakeTransition<FromStateType, ToStateType>(Convert.ToInt32(transitionCommand), transitionCondition);

    public void MakeTransition<FromStateType, ToStateType>(Func<State<StateOwner>, bool> transitionCondition)
        where FromStateType : State<StateOwner>
        where ToStateType : State<StateOwner>
        => MakeTransition<FromStateType, ToStateType>(StateTransition<StateOwner>.NullCommand, transitionCondition);

    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand)
        where FromStateType : State<StateOwner>
        where ToStateType : State<StateOwner>
        => MakeTransition<FromStateType, ToStateType>(transitionCommand, null);

    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand)
        where FromStateType : State<StateOwner>
        where ToStateType : State<StateOwner>
        => MakeTransition<FromStateType, ToStateType>(transitionCommand, null);
    #endregion

    public bool ExecuteCommand(int transitionCommand)
    {
        // 현재 실행중인 CurrentStateData의 Transitions에서
        // enum 커맨드가 일치하고, 전이 조건을 만족하는 Transition을 찾아옴
        var transition = currentStateData.Transitions.FirstOrDefault(x 
            => x.TransitionCommand == transitionCommand && x.IsTransferable);

        // 적합한 Transtion을 찾아오지 못했다면 명령 실행은 실패
        if (transition == null)
            return false;

        // 적합한 Transiton을 찾아왔다면 해당 Transition의 ToState로 전이
        ChangeState(transition.ToState);
        return true;
    }
    #region ExecuteCommand Wrapping
    // ExecuteCommand의 Enum Command 버전
    public bool ExecuteCommand(Enum transitionCommand)
        => ExecuteCommand(Convert.ToInt32(transitionCommand));
    #endregion


    // 현재 실행중인 스테이트를 가져오기
    public State<StateOwner> GetCurrentState() => currentStateData.State;

    // 현재 실행중인 State의 Type을 가져옴
    public Type GetCurrentStateType() => GetCurrentState().GetType();
}
