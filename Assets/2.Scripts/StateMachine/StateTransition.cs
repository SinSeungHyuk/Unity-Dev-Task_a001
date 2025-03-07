using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class StateTransition<StateOwner>
{
    // Ʈ�������� Ŀ�ǵ尡 ������
    public const int NullCommand = int.MinValue;

    // ���̸� ���� ���� �Լ�
    // Func<T1,T2> : T1�� �Ű������� T2�� ��ȯ (Func �Լ��� ��� bool ��ȯ)
    private Func<State<StateOwner>, bool> transitionCondition;

    // ���� ������Ʈ
    public State<StateOwner> FromState { get; private set; }
    // FromState�κ��� ������ ������Ʈ
    public State<StateOwner> ToState { get; private set; }
    // ���� ��ɾ� (enum���� ����� ����)
    public int TransitionCommand { get; private set; }
    // ���� �������� ���� (�����Լ��� ���ų� FromState�� ���°� true�� ���)
    public bool IsTransferable => transitionCondition == null || transitionCondition.Invoke(FromState);


    // Ʈ������ ������
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
