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

    // State�� ������ ��Ƽ� ������ �̳�Ŭ����
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

    // <������Ʈ�� Ŭ���� Ÿ��, ������Ʈ ������>
    private readonly Dictionary<Type, StateData> stateDatas = new();

    private StateData currentStateData;

    public StateOwner StateMachineOwner { get; private set; }


    // MonoBehaviour�� ���� ������ ���� Update���� ����
    // �̰� ��ӹ޴� �ڽ� ������Ʈ�ӽ��� Update�� ���� ȣ��
    public void Update()
    {
        // ���� �������� ������Ʈ�� Ʈ������ ���ǵ��� �˻�
        TryTransition(currentStateData.Transitions);

        // �������� ���ߴٸ� ���� State�� Update�� ������
        currentStateData.State.Update();
    }

    public void InitializeStateMachine(StateOwner owner)
    {
        StateMachineOwner = owner;
    }

    public void SetUpDefaultState()
    {
        // �켱 ������ ���� ���� StateData�� ã�ƿ�
        StateData firstStateData = stateDatas.Values.FirstOrDefault(x => x.Priority == 0);
        // ã�ƿ� StateData�� State�� Current State�� ��������
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
        OnStateChanged?.Invoke(this, newStateData.State, prevState?.State);
    }

    private void ChangeState(State<StateOwner> newState)
    {
        // StateDatas �� newState�� ���� StateData�� ã�ƿ�
        StateData newStateData = stateDatas[newState.GetType()];
        ChangeState(newStateData);
    }

    private bool TryTransition(IReadOnlyList<StateTransition<StateOwner>> transtions)
    {
        foreach (var transition in transtions)
        {
            // enum Ŀ�ǵ尡 null�� �ƴϰų� Ʈ������ Func�� false�� ��� ���� ����
            if (transition.TransitionCommand != StateTransition<StateOwner>.NullCommand || !transition.IsTransferable)
                continue;

            // ��� ������ �����Ѵٸ� ToState�� ����
            ChangeState(transition.ToState);
            return true;
        }
        return false;
    }


    public void AddState<T>() where T : State<StateOwner>
    {
        // Type�� ���� State�� ����
        State<StateOwner> newState = Activator.CreateInstance<T>();
        newState.SetUp(this, StateMachineOwner);

        // AddState�� ȣ���ϴ� ������ �� ������Ʈ�� �켱����
        stateDatas.Add(typeof(T), new StateData(stateDatas.Count, newState));
    }

    // FromStateType�� ���� State�� Type
    // ToStateType�� ������ State�� Type
    // �� Tpye ��� State<Owner> class�� �ڽ��̿�����
    public void MakeTransition<FromStateType, ToStateType>(int transitionCommand,
        Func<State<StateOwner>, bool> transitionCondition)
        where FromStateType : State<StateOwner>
        where ToStateType : State<StateOwner>
    {
        // StateDatas���� FromStateType�� State�� ���� StateData�� ã�ƿ�
        StateData fromStateData = stateDatas[typeof(FromStateType)];
        // StateDatas���� ToStateType�� State�� ���� StateData�� ã�ƿ�
        StateData toStateData = stateDatas[typeof(ToStateType)];

        var newTransition = new StateTransition<StateOwner>(fromStateData.State, toStateData.State,
            transitionCommand, transitionCondition);
        // ������ Transition�� FromStateData�� Transition���� �߰�
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
        // ���� �������� CurrentStateData�� Transitions����
        // enum Ŀ�ǵ尡 ��ġ�ϰ�, ���� ������ �����ϴ� Transition�� ã�ƿ�
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


    // ���� �������� ������Ʈ�� ��������
    public State<StateOwner> GetCurrentState() => currentStateData.State;

    // ���� �������� State�� Type�� ������
    public Type GetCurrentStateType() => GetCurrentState().GetType();
}
