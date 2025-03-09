using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ZombieStateMachine : MonsterStateMachine<Monster>
{
    protected override void AddStates()
    {
        AddState<MoveToTruckState>();
        AddState<JumpState>(); 
        AddState<AttackState>();
        AddState<MoveToBackwardState>();
    }

    protected override void MakeTransitions()
    {
        // �̵� -> ���� / ����
        MakeTransition<MoveToTruckState, JumpState>(EMonsterStateCommand.Jump);
        //MakeTransition<MoveToTruckState, AttackState>(EMonsterStateCommand.Attack, state => !(state as AttackState).IsAttack);
        MakeTransition<MoveToTruckState, AttackState>(EMonsterStateCommand.Attack);
        //MakeTransition<MoveToTruckState, MoveToBackwardState>(EMonsterStateCommand.MoveBackward);

        // ���� -> �̵� (������ ������ �̵��� �ѹ� ���ƴٰ� �������� ����)
        //MakeTransition<JumpState, MoveToTruckState>(EMonsterStateCommand.Move);
        MakeTransition<JumpState, MoveToTruckState>(state => (state as JumpState).IsJumping == false);

        // ���ݻ����� ���͸� �޹��� ���·� ���� ����
        MakeTransition<AttackState, MoveToBackwardState>(EMonsterStateCommand.MoveBackward);
        // �ڷ� �̵��� ������ �ٽ� �̵����·� ����

        MakeTransition<MoveToBackwardState, MoveToTruckState>(EMonsterStateCommand.Move);
    }

    public bool IsMoveToTruckState()
        => GetCurrentStateType() == typeof(MoveToTruckState);

    public bool IsJumpState()
        => GetCurrentStateType() == typeof(JumpState);

    public bool IsAttackState()
    => GetCurrentStateType() == typeof(AttackState);
}
