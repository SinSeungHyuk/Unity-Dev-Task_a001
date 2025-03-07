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
        MakeTransition<MoveToTruckState, JumpState>(EMonsterStateCommand.Jump);
        MakeTransition<MoveToTruckState, AttackState>(EMonsterStateCommand.Attack);
        MakeTransition<MoveToTruckState, MoveToBackwardState>(EMonsterStateCommand.MoveBackward);

        MakeTransition<JumpState, MoveToTruckState>(state => (state as JumpState).IsJumping == false);
        MakeTransition<JumpState, AttackState>(EMonsterStateCommand.Attack); // ������ ������ ���� Ʈ���� �ε��� �� ����

        MakeTransition<AttackState, MoveToTruckState>(EMonsterStateCommand.Move);

    }
}
