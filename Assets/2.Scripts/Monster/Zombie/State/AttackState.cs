using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class AttackState : State<Monster>
{
    private Monster monster;
    private MonsterCtrl monsterCtrl;

    private bool isAttack;

    public bool IsAttack => isAttack;


    protected override void Awake()
    {
        monster = StateMachineOwner;
        monsterCtrl = StateMachineOwner.MonsterCtrl;
    }

    public override void Enter()
    {
        // ���� �ִϸ��̼� ���
        StateMachineOwner.Animator.SetBool("IsAttacking", true);
        isAttack = true;
    }

    public override void Update()
    {
        // �������϶��� ��� ������ ���ư����� (Ʈ���� ���� velocity�� ���ϹǷ�)
        monsterCtrl.Rigid.velocity = Vector2.left * monster.Speed;
    }

    public override void Exit()
    {
        // ���� �ִϸ��̼� ����
        StateMachineOwner.Animator.SetBool("IsAttacking", false);
        isAttack = false;
    }
}