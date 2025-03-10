using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class State<StateOwner>
{
    // �� ������ ������Ʈ�ӽŰ� ������Ʈ�ӽ��� ������(����)
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
}
