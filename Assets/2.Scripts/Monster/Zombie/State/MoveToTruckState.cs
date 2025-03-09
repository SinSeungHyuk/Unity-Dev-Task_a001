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
        // ���÷� Enter�� ó�� �ѹ��� �ٲٸ� ���Ŀ� Ʈ���� ���� velocity�� ���Ҽ��� �����Ƿ� ������Ʈ
        monsterCtrl.Rigid.velocity = Vector2.left * monster.Speed;
    }

    public override void Exit()
    {
    }
}