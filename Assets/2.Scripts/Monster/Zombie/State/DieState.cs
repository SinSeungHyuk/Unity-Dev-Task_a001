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
        // 사망 애니메이션 재생
        monster.Animator.SetBool("IsDead", true);
        monster.Hitbox.enabled = false;
        monsterCtrl.Rigid.isKinematic = true;
    }

    public override void Exit()
    {
        // 몬스터가 다시 스폰될때 SetUpDefaultState()에 의해 이동상태로 전이되면서 Exit 호출됨
        monster.Hitbox.enabled = true;
        monsterCtrl.Rigid.isKinematic = false;
    }
}