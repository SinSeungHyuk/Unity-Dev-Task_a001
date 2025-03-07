using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class AttackState : State<Monster>
{
    public override void Enter()
    {
        StateMachineOwner.Animator.SetBool("IsAttacking", true);
        
        StateMachine.ExecuteCommand(EMonsterStateCommand.Move);

    }

    public override void Exit()
    {
        Debug.Log("공격종료 ");
    }
}