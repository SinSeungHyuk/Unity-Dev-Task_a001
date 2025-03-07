using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class State<StateOwner>
{
    // TOwner�� State �������� Type (Skill, Entity ...)
    // StateMachine�� TOwner�� ��ġ�ؾ���
    public StateMachine<StateOwner> StateMachine { get; private set; }
    public StateOwner StateMachineOwner { get; private set; }

    // ������Ʈ�ӽſ��� ����� �Լ�
    public void SetUp(StateMachine<StateOwner> owner, StateOwner type)
    {
        StateMachine = owner;
        StateMachineOwner = type;

        Awake();
    }

    // �� ������Ʈ�� ���� �ʱ�ȭ �۾��� ������ Awake ���ҿ�
    protected virtual void Awake() { }

    // State�� ���۵ɶ�
    public virtual void Enter() { }
    // State �������϶� �����Ӹ���
    public virtual void Update() { }
    // State ������
    public virtual void Exit() { }

    //// StateMachine Ŭ������ SendMessage �Լ��� ȣ���� �Լ�
    //// ������Ʈ�ӽſ��� �� ������Ʈ�� Ư�� �޼���(enum)�� ������ ���
    //public virtual bool OnReceiveMessage(int message, object data)
    //    => false;
}
