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
        monsterCtrl.Rigid.velocity = Vector2.left * monster.Speed;
    }

    public override void Update()
    {
        // 예시로 Enter로 처음 한번만 바꾸면 이후에 트럭에 의해 velocity가 변할수도 있으므로 업데이트
        monsterCtrl.Rigid.velocity = Vector2.left * monster.Speed;
    }

    public override void Exit()
    {
    }
}