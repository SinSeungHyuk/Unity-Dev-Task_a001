using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveToTruckState : State<Monster>
{
    private Monster monster;
    private MonsterCtrl monsterCtrl;


    protected override void Awake()
    {
        monster = StateMachineOwner;
        monsterCtrl = StateMachineOwner.MonsterCtrl;
    }
    public override void Enter()
    {
        //monsterCtrl.SetDirX(-1f * monster.Speed);
        monsterCtrl.Rigid.velocity = Vector2.left * monster.Speed;
    }

    public override void Exit()
    {
        //Debug.Log("¹«ºù ³¡!");
    }
}