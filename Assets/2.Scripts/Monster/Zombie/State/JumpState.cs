using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class JumpState : State<Monster>
{
    private MonsterCtrl monsterCtrl;
    private bool isJumping = true;

    public bool IsJumping => isJumping;


    protected override void Awake()
    {
        monsterCtrl = StateMachineOwner.MonsterCtrl;
    }

    public override void Enter()
    {
        monsterCtrl.Jump();
        isJumping = true;
        //StateMachine.ExecuteCommand(EMonsterStateCommand.Move);
    }

    public override void Update()
    {
        isJumping = (monsterCtrl.Rigid.velocity.y > 0) ? true : false;
    }

    public override void Exit()
    {
        Debug.Log("Á¡ÇÁ ¹þ¾î³²");
    }
}