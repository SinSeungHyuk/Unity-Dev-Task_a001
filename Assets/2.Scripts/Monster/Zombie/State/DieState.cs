using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieState : State<Monster>
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
        // ��� �ִϸ��̼� ���
        monster.Animator.SetBool("IsDead", true);
        monster.Hitbox.enabled = false;
        monsterCtrl.Rigid.isKinematic = true;
    }

    public override void Exit()
    {
        // ���Ͱ� �ٽ� �����ɶ� SetUpDefaultState()�� ���� �̵����·� ���̵Ǹ鼭 Exit ȣ���
        monster.Hitbox.enabled = true;
        monsterCtrl.Rigid.isKinematic = false;
    }
}