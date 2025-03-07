using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToBackwardState : State<Monster>
{
    private MonsterCtrl monsterCtrl;


    protected override void Awake()
    {
        monsterCtrl = StateMachineOwner.MonsterCtrl;
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {
       
        StateMachine.ExecuteCommand(EMonsterStateCommand.Move);
    }
}
