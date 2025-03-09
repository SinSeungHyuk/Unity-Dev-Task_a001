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
        // 공격 애니메이션 재생
        StateMachineOwner.Animator.SetBool("IsAttacking", true);
        isAttack = true;
    }

    public override void Update()
    {
        // 공격중일때도 계속 앞으로 나아가야함 (트럭에 의해 velocity가 변하므로)
        monsterCtrl.Rigid.velocity = Vector2.left * monster.Speed;
    }

    public override void Exit()
    {
        // 공격 애니메이션 종료
        StateMachineOwner.Animator.SetBool("IsAttacking", false);
        isAttack = false;
    }
}