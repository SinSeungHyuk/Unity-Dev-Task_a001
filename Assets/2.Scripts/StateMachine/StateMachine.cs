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
    // 스테이트 변경될때 호출될 이벤트
    // <스테이트머신, 새 스테이트, 이전 스테이트>
    public event Action
        <StateMachine<StateOwner>, State<StateOwner>, State<StateOwner>> OnStateChanged;

    // State의 정보를 담아서 관리할 inner클래스
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

    // Layer별 가지고 있는 StateDatas(=Layer Dictionary), Dictionary의 Key는 Value인 StateData가 가진 State의 Type
    // 즉, State의 Type을 통해 해당 State를 가진 StateData를 찾아올 수 있음
    private readonly Dictionary<Type, StateData> stateDatas = new();

    // Layer별 Any Tansitions(조건만 만족하면 언제든지 ToState로 전이되는 Transition)
    //private readonly Dictionary<int, List<StateTransition<StateOwner>>> anyTransitionsByLayer = new();

    // Layer별 현재 실행중인 StateData(=현재 실행중인 State)
    private StateData currentStateData;

    // StateMachine의 소유주 (Player, Skill)
    public StateOwner StateMachineOwner { get; private set; }


    // MonoBehaviour가 없기 때문에 실제 Update되진 않음
    // 이걸 상속받는 자식 스테이트머신의 Update로 수동 호출
    public void Update()
    {
        // Layer가 가진 AnyTransitions를 찾아옴
        //bool hasAnyTransitions = anyTransitionsByLayer.TryGetValue(layer, out var anyTransitions);

        // AnyTansition이 존재하면다면 AnyTransition통해 ToState 전이를 시도하고,
        // 조건이 맞지 않아 전이하지 않았다면, 현재 StateData의 Transition을 이용해 전이를 시도함
        //if ((hasAnyTransitions && TryTransition(anyTransitions, layer)) ||
        //    TryTransition(currentStateData.Transitions, layer))
        //    continue;

        // 현재 실행중인 스테이트의 트랜지션 조건들을 검사
        TryTransition(currentStateData.Transitions);

        // 전이하지 못했다면 현재 State의 Update를 실행함
        currentStateData.State.Update();
        
    }

    public void InitializeStateMachine(StateOwner owner)
    {
        StateMachineOwner = owner;
    }

    // Layer별로 Current State를 설정해주는 해주는 함수
    public void SetUpDefaultState()
    {
        // 우선 순위가 가장 높은 StateData를 찾아옴
        StateData firstStateData = stateDatas.Values.FirstOrDefault(x => x.Priority == 0);
        // 찾아온 StateData의 State를 현재 Layer의 Current State로 설정해줌
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
        OnStateChanged?.Invoke(this, newStateData.State, prevState.State);
    }

    // newState의 Type을 이용해 StateData를 찾아와서 현재 실행중인 CurrentStateData를 변경하는 함수
    private void ChangeState(State<StateOwner> newState)
    {
        // Layer에 저장된 StateDatas중 newState를 가진 StateData를 찾아옴
        StateData newStateData = stateDatas[newState.GetType()];
        ChangeState(newStateData);
    }

    private bool TryTransition(IReadOnlyList<StateTransition<StateOwner>> transtions)
    {
        foreach (var transition in transtions)
        {
            // Command가 존재한다면, Command를 받았을 때만 전이 시도를 해야함으로 넘어감
            // Command가 존재하지 않아도, 전이 조건을 만족하지 못하면 넘어감
            if (transition.TransitionCommand != StateTransition<StateOwner>.NullCommand || !transition.IsTransferable)
                continue;

            // 모든 조건을 만족한다면 ToState로 전이
            ChangeState(transition.ToState);
            return true;
        }
        return false;
    }


    // Generic을 통해 StateMachine에 State를 추가하는 함수
    // T는 State<TOwnerType> class를 상속받은 Type이여야함
    public void AddState<T>() where T : State<StateOwner>
    {
        // Type을 통해 State를 생성
        State<StateOwner> newState = Activator.CreateInstance<T>();
        newState.SetUp(this, StateMachineOwner);

        // AddState를 호출하는 순서가 곧 스테이트의 우선순위
        stateDatas.Add(typeof(T), new StateData(stateDatas.Count, newState));
    }

    // Transition을 생성하는 함수
    // FromStateType은 현재 State의 Type
    // ToStateType은 전이할 State의 Type
    // 두 Tpye 모두 State<TOwnerType> class를 자식이여야함
    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand,
        Func<State<StateOwner>, bool> transitionCondition)
        where FromStateType : State<StateOwner>
        where ToStateType : State<StateOwner>
    {
        // StateDatas에서 FromStateType의 State를 가진 StateData를 찾아옴
        StateData fromStateData = stateDatas[typeof(FromStateType)];
        // StateDatas에서 ToStateType의 State를 가진 StateData를 찾아옴
        StateData toStateData = stateDatas[typeof(ToStateType)];

        // 인자와 찾아온 Data를 가지고 Transition을 생성
        // AnyTransition이 아닌 일반 Transition은 canTransitionToSelf 인자가 무조건 true
        var newTransition = new StateTransition<StateOwner>(fromStateData.State, toStateData.State,
            transitionCommand, transitionCondition);
        // 생성한 Transition을 FromStateData의 Transition으로 추가
        fromStateData.Transitions.Add(newTransition);
    }
    #region Make Transition Wrapping
    // MakeTransition 함수의 Enum Command 버전
    // Enum형으로 받은 Command를 Int로 변환하여 위의 함수를 호출함
    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand,
        Func<State<StateOwner>, bool> transitionCondition)
        where FromStateType : State<StateOwner>
        where ToStateType : State<StateOwner>
        => MakeTransition<FromStateType, ToStateType>(Convert.ToInt32(transitionCommand), transitionCondition);

    // Enum 커맨드는 Null, 대신 Func를 통해 전이조건을 검사하는 버전
    public void MakeTransition<FromStateType, ToStateType>(Func<State<StateOwner>, bool> transitionCondition)
        where FromStateType : State<StateOwner>
        where ToStateType : State<StateOwner>
        => MakeTransition<FromStateType, ToStateType>(StateTransition<StateOwner>.NullCommand, transitionCondition);

    // MakeTransition 함수의 Condition 인자가 없는 버전
    // Condition으로 null을 넣어서 최상단의 MakeTransition 함수를 호출함 
    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand)
        where FromStateType : State<StateOwner>
        where ToStateType : State<StateOwner>
        => MakeTransition<FromStateType, ToStateType>(transitionCommand, null);

    // 위 함수의 Enum 버전(Command 인자가 Enum형이고 Condition 인자가 없음)
    // 위에 정의된 Enum버전 MakeTransition 함수를 호출함
    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand)
        where FromStateType : State<StateOwner>
        where ToStateType : State<StateOwner>
        => MakeTransition<FromStateType, ToStateType>(transitionCommand, null);
    #endregion

    //// AnyTransition을 만드는 함수
    //// 조건을 만족하면 ToStateType 스테이트로 무조건 바로 전이되어야함
    //public void MakeAnyTransition<ToStateType>(int transitionCommand,
    //    Func<State<StateOwner>, bool> transitionCondition, int layer = 0, bool canTransitonToSelf = false)
    //    where ToStateType : State<StateOwner>
    //{
    //    var stateDatasByType = stateDatas[layer];
    //    // StateDatas에서 ToStateType의 State를 가진 StateData를 찾아옴
    //    var state = stateDatasByType[typeof(ToStateType)].State;
    //    // Transition 생성, 언제든지 조건만 맞으면 전이할 것이므로 FromState는 존재하지 않음
    //    var newTransition = new StateTransition<StateOwner>(null, state, transitionCommand, transitionCondition, canTransitonToSelf);
    //    // Layer의 AnyTransition으로 추가
    //    anyTransitionsByLayer[layer].Add(newTransition);
    //}
    //#region Make AnyTransition Wrapping
    //// MakeAnyTransition 함수의 Enum Command 버전
    //// Enum형으로 받은 Command를 Int로 변환하여 위의 함수를 호출함
    //public void MakeAnyTransition<ToStateType>(Enum transitionCommand,
    //    Func<State<StateOwner>, bool> transitionCondition, int layer = 0, bool canTransitonToSelf = false)
    //    where ToStateType : State<StateOwner>
    //    => MakeAnyTransition<ToStateType>(Convert.ToInt32(transitionCommand), transitionCondition, layer, canTransitonToSelf);

    //// MakeAnyTransition 함수의 Command 인자가 없는 버전
    //// NullCommand를 넣어서 최상단의 MakeTransition 함수를 호출함
    //public void MakeAnyTransition<ToStateType>(Func<State<StateOwner>, bool> transitionCondition,
    //    int layer = 0, bool canTransitonToSelf = false)
    //    where ToStateType : State<StateOwner>
    //    => MakeAnyTransition<ToStateType>(StateTransition<StateOwner>.NullCommand, transitionCondition, layer, canTransitonToSelf);

    //// MakeAnyTransiiton의 Condition 인자가 없는 버전
    //// Condition으로 null을 넣어서 최상단의 MakeTransition 함수를 호출함 
    //public void MakeAnyTransition<ToStateType>(int transitionCommand, int layer = 0, bool canTransitonToSelf = false)
    //where ToStateType : State<StateOwner>
    //    => MakeAnyTransition<ToStateType>(transitionCommand, null, layer, canTransitonToSelf);

    //// 위 함수의 Enum 버전(Command 인자가 Enum형이고 Condition 인자가 없음)
    //// 위에 정의된 Enum버전 MakeAnyTransition 함수를 호출함
    //public void MakeAnyTransition<ToStateType>(Enum transitionCommand, int layer = 0, bool canTransitonToSelf = false)
    //    where ToStateType : State<StateOwner>
    //    => MakeAnyTransition<ToStateType>(transitionCommand, null, layer, canTransitonToSelf);
    //#endregion

    // Command를 받아서 Transition을 실행하는 함수
    public bool ExecuteCommand(int transitionCommand)
    {
        // 현재 실행중인 CurrentStateData의 Transitions에서
        // Command가 일치하고, 전이 조건을 만족하는 Transition을 찾아옴
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

    //// 현재 실행중인 CurrentStateData로 Message를 보내는 함수
    //// 스테이트머신에서 메세지를 보내면 그 스테이트의 OnReceiveMessage 함수 호출
    //public bool SendMessage(int message, int layer, object extraData = null)
    //    => currentStateData[layer].State.OnReceiveMessage(message, extraData);
    //#region SendMessage Wrapping
    //// SendMessage 함수의 Enum Message 버전
    //public bool SendMessage(Enum message, int layer, object extraData = null)
    //    => SendMessage(Convert.ToInt32(message), layer, extraData);

    //// 모든 Layer의 현재 실행중인 CurrentStateData를 대상으로 SendMessage 함수를 실행하는 함수
    //// 하나의 CurrentStateData라도 적절한 Message를 수신했다면 true를 반환
    //public bool SendMessage(int message, object extraData = null)
    //{
    //    bool isSuccess = false;
    //    foreach (int layer in layers)
    //    {
    //        if (SendMessage(message, layer, extraData))
    //            isSuccess = true;
    //    }
    //    return isSuccess;
    //}
    //// 위 SendMessage 함수의 Enum Message 버전
    //public bool SendMessage(Enum message, object extraData = null)
    //    => SendMessage(Convert.ToInt32(message), extraData);
    //#endregion

    //// 모든 Layer의 현재 실행중인 CurrentState를 확인하여, 현재 State가 T Type의 State인지 확인하는 함수
    //// CurrentState가 T Type인게 확인되면 즉시 true를 반환함
    //public bool IsInState<T>() where T : State<StateOwner>
    //{
    //    foreach ((_, StateData data) in currentStateData)
    //    {
    //        // ex) if (IsInState<State<InActionState>>) 
    //        // => 만약 현재 실행중인 스테이트가 InActionState 스테이트라면 true
    //        if (data.State.GetType() == typeof(T))
    //            return true;
    //    }
    //    return false;
    //}

    // 현재 실행중인 스테이트를 가져오기
    public State<StateOwner> GetCurrentState() => currentStateData.State;

    // 현재 실행중인 State의 Type을 가져옴
    public Type GetCurrentStateType() => GetCurrentState().GetType();
}
