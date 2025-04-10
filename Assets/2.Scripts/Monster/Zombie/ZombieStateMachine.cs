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
        AddState<DieState>();
    }

    protected override void MakeTransitions()
    {
        // 이동 -> 점프 / 공격
        MakeTransition<MoveToTruckState, JumpState>(EMonsterStateCommand.Jump);
        MakeTransition<MoveToTruckState, AttackState>(EMonsterStateCommand.Attack);

        // 점프 -> 이동 (점프가 끝나고 이동을 한번 거쳤다가 공격으로 전이)
        MakeTransition<JumpState, MoveToTruckState>(state => (state as JumpState).IsJumping == false);

        // 공격상태인 몬스터만 뒷무빙 상태로 전이 가능
        MakeTransition<AttackState, MoveToBackwardState>(EMonsterStateCommand.MoveBackward);

        // 뒤로 이동이 끝나면 다시 이동상태로 전이
        MakeTransition<MoveToBackwardState, MoveToTruckState>(EMonsterStateCommand.Move);

        // 어떤 상태에서도 사망 상태로 전이가능
        MakeTransition<MoveToTruckState, DieState>(EMonsterStateCommand.Die);
        MakeTransition<JumpState, DieState>(EMonsterStateCommand.Die);
        MakeTransition<AttackState, DieState>(EMonsterStateCommand.Die);
        MakeTransition<MoveToBackwardState, DieState>(EMonsterStateCommand.Die);

        MakeTransition<DieState, MoveToTruckState>(EMonsterStateCommand.Move);
    }

    public bool IsMoveToTruckState()
        => GetCurrentStateType() == typeof(MoveToTruckState);

    public bool IsJumpState()
        => GetCurrentStateType() == typeof(JumpState);

    public bool IsAttackState()
    => GetCurrentStateType() == typeof(AttackState);
}
