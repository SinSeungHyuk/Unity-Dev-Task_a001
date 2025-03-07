using System;
using System.Collections.Generic;
using UnityEngine;

// ���Ͱ� ��ӹ��� ������Ʈ�ӽ�
public abstract class MonsterStateMachine<Owner> : MonoBehaviour
{
    /// <summary>
    /// �߻�Ŭ������ ������Ʈ�ӽ��� �ϳ� �� ����� ����
    /// -> �Ϲ�Ŭ������ stateMachine ��ü�� �����ؼ� stateMachine�� �Լ��� ȣ���ؾ� �ϴµ�
    /// �Ź� stateMachine�� �ٿ��� �Լ��� ȣ���ϱ⿡ ���ŷӰ� �������� �������� ����
    /// ��, �� �߻�Ŭ������ Wrapping�� ���� ���� (+ Update,SetUp �� �ڵ��� �ߺ� ���̱�)
    /// </summary>


    // ������Ʈ ����ɶ� ȣ��� �̺�Ʈ
    // <������Ʈ�ӽ�, �� ������Ʈ, ���� ������Ʈ, ���̾�>
    public event Action<StateMachine<Owner>, State<Owner>, State<Owner>> OnStateChanged;

    private readonly StateMachine<Owner> stateMachine = new();

    public Owner StateMachineOwner => stateMachine.StateMachineOwner;


    private void Update()
    {
        // StateMachine Ŭ������ MonoBehaviour�� ��� ������Ʈ�� ȣ���� �ȵ�
        // EntityStateMachine�� ��ӹ޾� ������� ��¥ ������Ʈ�ӽ��� ������Ʈ���� ������Ʈ
        if (this.StateMachineOwner != null)
            stateMachine.Update();
    }

    public void InitializeStateMachine(Owner owner)
    {
        // 1. �����̸ӽ� �ʱ�ȭ -> owner(monster)�� �־ ������Ʈ�ӽ��� ������ �˷���
        // 2. ������Ʈ�� Ʈ�����ǵ� �߰� -> ��ӹ��� �ڽ�Ŭ�������� �����Ѵ�� ����,Ʈ�������� �߰�����
        // 3. ���¿� Ʈ�������� �߰��Ǹ鼭 ������Ʈ�ӽ��� ������ �ִ� ������ �������� ������
        // 4. ��ó�� ���۵Ǿ���ϴ� �⺻���� ����

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


    // �� ������Ʈ�ӽ��� ��ӹ޾� ����ϰ� �� '��¥ ������Ʈ�ӽ�'���� ����
    protected abstract void AddStates();
    protected abstract void MakeTransitions();
}
