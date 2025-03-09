using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class JumpState : State<Monster>
{
    private MonsterCtrl monsterCtrl;
    private bool isJumping;

    public bool IsJumping => isJumping;


    protected override void Awake()
    {
        monsterCtrl = StateMachineOwner.MonsterCtrl;
    }

    public override void Enter()
    {
        isJumping = true;
        monsterCtrl.Jump();
    }

    public override void Update()
    {
        isJumping = monsterCtrl.IsJump;
    }

    public override void Exit()
    {
        isJumping = false;
    }
}