using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> sprites;

    public MonsterCtrl MonsterCtrl {  get; private set; }
    public ZombieStateMachine StateMachine { get; private set; }
    public MonsterDetailsSO MonsterData { get; private set; }
    public Animator Animator { get; private set; }
    public float Speed {  get; private set; }



    private void Awake()
    {
        MonsterCtrl = GetComponent<MonsterCtrl>();
        Animator = GetComponent<Animator>();
    }

    public void InitializeMonster(MonsterDetailsSO monsterData, LayerMask lane)
    {
        sprites.Zip(monsterData.sprites, (sprite, dataSprite) => sprite.sprite = dataSprite).ToList();

        MonsterData = monsterData;
        SetLayerRecursively(gameObject, lane); // 상,중,하단 중 각자 스폰된 라인에 맞는 레이어 적용
        Speed = monsterData.speed;

        // 임의로 ZombieStateMachine를 넣어줌 => 원래라면 monsterData를 통해 적절한 상태머신을 받아와야함
        StateMachine = GetComponent<ZombieStateMachine>();
        StateMachine.InitializeStateMachine(this);
    }

    void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    private void OnAttack()
    {
        // 공격 함수
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.TryGetComponent<Truck>(out var truck))
    //    {
    //        // 트럭과 충돌시 공격상태로 전이
    //        StateMachine.ExecuteCommand(EMonsterStateCommand.Attack);
    //    }

    //    // 같은 레이어 오브젝트끼리 부딪혔을때 (같은 라인에 있는 몬스터)
    //    if (collision.gameObject.layer == this.gameObject.layer)
    //    {
    //        if (CheckCollisionDir(collision.gameObject) == ECollisionDir.Top)
    //            StateMachine.ExecuteCommand(EMonsterStateCommand.MoveBackward);
    //    }

    //    //if (StateMachine.IsJumpState())
    //    //    StateMachine.ExecuteCommand(EMonsterStateCommand.Move); // 점프중일때 충돌하면 점프 종료 
    //}

    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    // 다른 상태에서 트럭과 충돌을 이미 했어도 공격상태로 전이되야하므로 Stay에서 검사
    //    if (collision.gameObject.TryGetComponent<Truck>(out var truck))
    //    {
    //        StateMachine.ExecuteCommand(EMonsterStateCommand.Attack);
    //    }

    //    if (collision.gameObject.layer == this.gameObject.layer)
    //    {
    //        if (CheckCollisionDir(collision.gameObject) == ECollisionDir.Top)
    //            StateMachine.ExecuteCommand(EMonsterStateCommand.MoveBackward);
    //    }
    //}

    //private void OnTriggerStay2D(Collider2D collision)
    //{
    //    if (collision.gameObject.layer == this.gameObject.layer)
    //    {
    //        if (CheckCollisionDir(collision.gameObject) == ECollisionDir.Left)
    //            StateMachine.ExecuteCommand(EMonsterStateCommand.Jump);
    //    }
    //}

    //private ECollisionDir CheckCollisionDir(GameObject other)
    //{
    //    Vector2 dir = (other.transform.position - this.transform.position).normalized;

    //    float dirX = Mathf.Abs(dir.x);
    //    float dirY = Mathf.Abs(dir.y);

    //    if (dirX > dirY)
    //    {
    //        if (dir.x < 0)
    //            return ECollisionDir.Left;
    //    }
    //    else if (dirX < dirY)
    //    {
    //        if (dir.y > 0)
    //            return ECollisionDir.Top;
    //    }

    //    return ECollisionDir.NoneDir;
    //}
}
