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
    // ������Ʈ ����ɶ� ȣ��� �̺�Ʈ
    // <������Ʈ�ӽ�, �� ������Ʈ, ���� ������Ʈ>
    public event Action
        <StateMachine<StateOwner>, State<StateOwner>, State<StateOwner>> OnStateChanged;

    // State�� ������ ��Ƽ� ������ innerŬ����
    private class StateData
    {
        // ������Ʈ�� ��� ���� (����� ������� �켱���� �ο�)
        public int Priority { get; private set; }
        // �� data�� ���� ������Ʈ
        public State<StateOwner> State { get; private set; }
        // �� ������Ʈ�� ����� Ʈ�����ǵ�
        public List<StateTransition<StateOwner>> Transitions { get; private set; } = new();

        // StateData ������
        public StateData(int priority, State<StateOwner> state)
        {
            Priority = priority;
            State = state;
        }
    }

    // Layer�� ������ �ִ� StateDatas(=Layer Dictionary), Dictionary�� Key�� Value�� StateData�� ���� State�� Type
    // ��, State�� Type�� ���� �ش� State�� ���� StateData�� ã�ƿ� �� ����
    private readonly Dictionary<Type, StateData> stateDatas = new();

    // Layer�� Any Tansitions(���Ǹ� �����ϸ� �������� ToState�� ���̵Ǵ� Transition)
    //private readonly Dictionary<int, List<StateTransition<StateOwner>>> anyTransitionsByLayer = new();

    // Layer�� ���� �������� StateData(=���� �������� State)
    private StateData currentStateData;

    // StateMachine�� ������ (Player, Skill)
    public StateOwner StateMachineOwner { get; private set; }


    // MonoBehaviour�� ���� ������ ���� Update���� ����
    // �̰� ��ӹ޴� �ڽ� ������Ʈ�ӽ��� Update�� ���� ȣ��
    public void Update()
    {
        // Layer�� ���� AnyTransitions�� ã�ƿ�
        //bool hasAnyTransitions = anyTransitionsByLayer.TryGetValue(layer, out var anyTransitions);

        // AnyTansition�� �����ϸ�ٸ� AnyTransition���� ToState ���̸� �õ��ϰ�,
        // ������ ���� �ʾ� �������� �ʾҴٸ�, ���� StateData�� Transition�� �̿��� ���̸� �õ���
        //if ((hasAnyTransitions && TryTransition(anyTransitions, layer)) ||
        //    TryTransition(currentStateData.Transitions, layer))
        //    continue;

        // ���� �������� ������Ʈ�� Ʈ������ ���ǵ��� �˻�
        TryTransition(currentStateData.Transitions);

        // �������� ���ߴٸ� ���� State�� Update�� ������
        currentStateData.State.Update();
        
    }

    public void InitializeStateMachine(StateOwner owner)
    {
        StateMachineOwner = owner;
    }

    // Layer���� Current State�� �������ִ� ���ִ� �Լ�
    public void SetUpDefaultState()
    {
        // �켱 ������ ���� ���� StateData�� ã�ƿ�
        StateData firstStateData = stateDatas.Values.FirstOrDefault(x => x.Priority == 0);
        // ã�ƿ� StateData�� State�� ���� Layer�� Current State�� ��������
        ChangeState(firstStateData);
    }

    // ���� �������� CurrentStateData�� �����ϴ� �Լ�
    private void ChangeState(StateData newStateData)
    {
        // ���� �������� CurrentStateData�� ������
        StateData prevState = currentStateData;

        prevState?.State.Exit();
        // ���� �������� CurrentStateData�� ���ڷ� ���� newStateData�� ��ü����
        currentStateData = newStateData;
        newStateData.State.Enter();

        // State�� ���̵Ǿ����� �˸�
        OnStateChanged?.Invoke(this, newStateData.State, prevState.State);
    }

    // newState�� Type�� �̿��� StateData�� ã�ƿͼ� ���� �������� CurrentStateData�� �����ϴ� �Լ�
    private void ChangeState(State<StateOwner> newState)
    {
        // Layer�� ����� StateDatas�� newState�� ���� StateData�� ã�ƿ�
        StateData newStateData = stateDatas[newState.GetType()];
        ChangeState(newStateData);
    }

    private bool TryTransition(IReadOnlyList<StateTransition<StateOwner>> transtions)
    {
        foreach (var transition in transtions)
        {
            // Command�� �����Ѵٸ�, Command�� �޾��� ���� ���� �õ��� �ؾ������� �Ѿ
            // Command�� �������� �ʾƵ�, ���� ������ �������� ���ϸ� �Ѿ
            if (transition.TransitionCommand != StateTransition<StateOwner>.NullCommand || !transition.IsTransferable)
                continue;

            // ��� ������ �����Ѵٸ� ToState�� ����
            ChangeState(transition.ToState);
            return true;
        }
        return false;
    }


    // Generic�� ���� StateMachine�� State�� �߰��ϴ� �Լ�
    // T�� State<TOwnerType> class�� ��ӹ��� Type�̿�����
    public void AddState<T>() where T : State<StateOwner>
    {
        // Type�� ���� State�� ����
        State<StateOwner> newState = Activator.CreateInstance<T>();
        newState.SetUp(this, StateMachineOwner);

        // AddState�� ȣ���ϴ� ������ �� ������Ʈ�� �켱����
        stateDatas.Add(typeof(T), new StateData(stateDatas.Count, newState));
    }

    // Transition�� �����ϴ� �Լ�
    // FromStateType�� ���� State�� Type
    // ToStateType�� ������ State�� Type
    // �� Tpye ��� State<TOwnerType> class�� �ڽ��̿�����
    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand,
        Func<State<StateOwner>, bool> transitionCondition)
        where FromStateType : State<StateOwner>
        where ToStateType : State<StateOwner>
    {
        // StateDatas���� FromStateType�� State�� ���� StateData�� ã�ƿ�
        StateData fromStateData = stateDatas[typeof(FromStateType)];
        // StateDatas���� ToStateType�� State�� ���� StateData�� ã�ƿ�
        StateData toStateData = stateDatas[typeof(ToStateType)];

        // ���ڿ� ã�ƿ� Data�� ������ Transition�� ����
        // AnyTransition�� �ƴ� �Ϲ� Transition�� canTransitionToSelf ���ڰ� ������ true
        var newTransition = new StateTransition<StateOwner>(fromStateData.State, toStateData.State,
            transitionCommand, transitionCondition);
        // ������ Transition�� FromStateData�� Transition���� �߰�
        fromStateData.Transitions.Add(newTransition);
    }
    #region Make Transition Wrapping
    // MakeTransition �Լ��� Enum Command ����
    // Enum������ ���� Command�� Int�� ��ȯ�Ͽ� ���� �Լ��� ȣ����
    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand,
        Func<State<StateOwner>, bool> transitionCondition)
        where FromStateType : State<StateOwner>
        where ToStateType : State<StateOwner>
        => MakeTransition<FromStateType, ToStateType>(Convert.ToInt32(transitionCommand), transitionCondition);

    // Enum Ŀ�ǵ�� Null, ��� Func�� ���� ���������� �˻��ϴ� ����
    public void MakeTransition<FromStateType, ToStateType>(Func<State<StateOwner>, bool> transitionCondition)
        where FromStateType : State<StateOwner>
        where ToStateType : State<StateOwner>
        => MakeTransition<FromStateType, ToStateType>(StateTransition<StateOwner>.NullCommand, transitionCondition);

    // MakeTransition �Լ��� Condition ���ڰ� ���� ����
    // Condition���� null�� �־ �ֻ���� MakeTransition �Լ��� ȣ���� 
    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand)
        where FromStateType : State<StateOwner>
        where ToStateType : State<StateOwner>
        => MakeTransition<FromStateType, ToStateType>(transitionCommand, null);

    // �� �Լ��� Enum ����(Command ���ڰ� Enum���̰� Condition ���ڰ� ����)
    // ���� ���ǵ� Enum���� MakeTransition �Լ��� ȣ����
    public void MakeTransition<FromStateType, ToStateType>(Enum transitionCommand)
        where FromStateType : State<StateOwner>
        where ToStateType : State<StateOwner>
        => MakeTransition<FromStateType, ToStateType>(transitionCommand, null);
    #endregion

    //// AnyTransition�� ����� �Լ�
    //// ������ �����ϸ� ToStateType ������Ʈ�� ������ �ٷ� ���̵Ǿ����
    //public void MakeAnyTransition<ToStateType>(int transitionCommand,
    //    Func<State<StateOwner>, bool> transitionCondition, int layer = 0, bool canTransitonToSelf = false)
    //    where ToStateType : State<StateOwner>
    //{
    //    var stateDatasByType = stateDatas[layer];
    //    // StateDatas���� ToStateType�� State�� ���� StateData�� ã�ƿ�
    //    var state = stateDatasByType[typeof(ToStateType)].State;
    //    // Transition ����, �������� ���Ǹ� ������ ������ ���̹Ƿ� FromState�� �������� ����
    //    var newTransition = new StateTransition<StateOwner>(null, state, transitionCommand, transitionCondition, canTransitonToSelf);
    //    // Layer�� AnyTransition���� �߰�
    //    anyTransitionsByLayer[layer].Add(newTransition);
    //}
    //#region Make AnyTransition Wrapping
    //// MakeAnyTransition �Լ��� Enum Command ����
    //// Enum������ ���� Command�� Int�� ��ȯ�Ͽ� ���� �Լ��� ȣ����
    //public void MakeAnyTransition<ToStateType>(Enum transitionCommand,
    //    Func<State<StateOwner>, bool> transitionCondition, int layer = 0, bool canTransitonToSelf = false)
    //    where ToStateType : State<StateOwner>
    //    => MakeAnyTransition<ToStateType>(Convert.ToInt32(transitionCommand), transitionCondition, layer, canTransitonToSelf);

    //// MakeAnyTransition �Լ��� Command ���ڰ� ���� ����
    //// NullCommand�� �־ �ֻ���� MakeTransition �Լ��� ȣ����
    //public void MakeAnyTransition<ToStateType>(Func<State<StateOwner>, bool> transitionCondition,
    //    int layer = 0, bool canTransitonToSelf = false)
    //    where ToStateType : State<StateOwner>
    //    => MakeAnyTransition<ToStateType>(StateTransition<StateOwner>.NullCommand, transitionCondition, layer, canTransitonToSelf);

    //// MakeAnyTransiiton�� Condition ���ڰ� ���� ����
    //// Condition���� null�� �־ �ֻ���� MakeTransition �Լ��� ȣ���� 
    //public void MakeAnyTransition<ToStateType>(int transitionCommand, int layer = 0, bool canTransitonToSelf = false)
    //where ToStateType : State<StateOwner>
    //    => MakeAnyTransition<ToStateType>(transitionCommand, null, layer, canTransitonToSelf);

    //// �� �Լ��� Enum ����(Command ���ڰ� Enum���̰� Condition ���ڰ� ����)
    //// ���� ���ǵ� Enum���� MakeAnyTransition �Լ��� ȣ����
    //public void MakeAnyTransition<ToStateType>(Enum transitionCommand, int layer = 0, bool canTransitonToSelf = false)
    //    where ToStateType : State<StateOwner>
    //    => MakeAnyTransition<ToStateType>(transitionCommand, null, layer, canTransitonToSelf);
    //#endregion

    // Command�� �޾Ƽ� Transition�� �����ϴ� �Լ�
    public bool ExecuteCommand(int transitionCommand)
    {
        // ���� �������� CurrentStateData�� Transitions����
        // Command�� ��ġ�ϰ�, ���� ������ �����ϴ� Transition�� ã�ƿ�
        var transition = currentStateData.Transitions.FirstOrDefault(x 
            => x.TransitionCommand == transitionCommand && x.IsTransferable);

        // ������ Transtion�� ã�ƿ��� ���ߴٸ� ��� ������ ����
        if (transition == null)
            return false;

        // ������ Transiton�� ã�ƿԴٸ� �ش� Transition�� ToState�� ����
        ChangeState(transition.ToState);
        return true;
    }
    #region ExecuteCommand Wrapping
    // ExecuteCommand�� Enum Command ����
    public bool ExecuteCommand(Enum transitionCommand)
        => ExecuteCommand(Convert.ToInt32(transitionCommand));
    #endregion

    //// ���� �������� CurrentStateData�� Message�� ������ �Լ�
    //// ������Ʈ�ӽſ��� �޼����� ������ �� ������Ʈ�� OnReceiveMessage �Լ� ȣ��
    //public bool SendMessage(int message, int layer, object extraData = null)
    //    => currentStateData[layer].State.OnReceiveMessage(message, extraData);
    //#region SendMessage Wrapping
    //// SendMessage �Լ��� Enum Message ����
    //public bool SendMessage(Enum message, int layer, object extraData = null)
    //    => SendMessage(Convert.ToInt32(message), layer, extraData);

    //// ��� Layer�� ���� �������� CurrentStateData�� ������� SendMessage �Լ��� �����ϴ� �Լ�
    //// �ϳ��� CurrentStateData�� ������ Message�� �����ߴٸ� true�� ��ȯ
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
    //// �� SendMessage �Լ��� Enum Message ����
    //public bool SendMessage(Enum message, object extraData = null)
    //    => SendMessage(Convert.ToInt32(message), extraData);
    //#endregion

    //// ��� Layer�� ���� �������� CurrentState�� Ȯ���Ͽ�, ���� State�� T Type�� State���� Ȯ���ϴ� �Լ�
    //// CurrentState�� T Type�ΰ� Ȯ�εǸ� ��� true�� ��ȯ��
    //public bool IsInState<T>() where T : State<StateOwner>
    //{
    //    foreach ((_, StateData data) in currentStateData)
    //    {
    //        // ex) if (IsInState<State<InActionState>>) 
    //        // => ���� ���� �������� ������Ʈ�� InActionState ������Ʈ��� true
    //        if (data.State.GetType() == typeof(T))
    //            return true;
    //    }
    //    return false;
    //}

    // ���� �������� ������Ʈ�� ��������
    public State<StateOwner> GetCurrentState() => currentStateData.State;

    // ���� �������� State�� Type�� ������
    public Type GetCurrentStateType() => GetCurrentState().GetType();
}
