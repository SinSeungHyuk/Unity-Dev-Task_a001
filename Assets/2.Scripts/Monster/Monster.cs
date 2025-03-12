using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.U2D;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Monster : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> sprites;
    private LayerMask layerMask;
    private float hp; // 임시로 이곳에 hp 만들어줌

    public MonsterCtrl MonsterCtrl {  get; private set; }
    public ZombieStateMachine StateMachine { get; private set; }
    public MonsterDetailsSO MonsterData { get; private set; }
    public Animator Animator { get; private set; }
    public PolygonCollider2D Hitbox { get; private set; }
    public float Speed {  get; private set; }



    private void Awake()
    {
        MonsterCtrl = GetComponent<MonsterCtrl>();
        Animator = GetComponent<Animator>();
        Hitbox = GetComponent<PolygonCollider2D>();
        // 임의로 ZombieStateMachine를 넣어줌 => 원래라면 monsterData를 통해 적절한 상태머신을 받아와야함
        StateMachine = GetComponent<ZombieStateMachine>();
        StateMachine.InitializeStateMachine(this);
    }

    void FixedUpdate()
    {
        // 공격중일때만 뒷무빙 상태로 전이하므로 최적화를 위해 현재상태 검사
        if (StateMachine.IsAttackState() == true)
        {
            RaycastHit2D[] upHits = Physics2D.RaycastAll(transform.position, Vector2.up, 1f, layerMask);
            foreach (var hit in upHits)
            {
                if (hit.collider.gameObject == this.gameObject) continue;

                StateMachine.ExecuteCommand(EMonsterStateCommand.MoveBackward);
            }
        }

        // 이동중일때만 점프or공격 상태로 전이하므로 최적화를 위해 현재상태 검사
        if (StateMachine.IsMoveToTruckState() == true)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.left, 1f);
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject == this.gameObject) continue;

                if (hit.collider.gameObject.layer == this.gameObject.layer && !StateMachine.IsJumpState()) // 같은 라인의 몬스터
                {
                    StateMachine.ExecuteCommand(EMonsterStateCommand.Jump);
                }
                else if ((1 << hit.collider.gameObject.layer) == Settings.truckLayer)
                {
                    StateMachine.ExecuteCommand(EMonsterStateCommand.Attack);
                }
            }
        }
    }

    public void InitializeMonster(MonsterDetailsSO monsterData, int lane)
    {
        sprites.Zip(monsterData.sprites, (sprite, dataSprite) => sprite.sprite = dataSprite).ToList();

        SetLayerRecursively(gameObject, lane); // 상,중,하단 중 각자 스폰된 라인에 맞는 레이어 적용
        layerMask = 1 << lane; // 매개변수로 받은건 단일 레이어. 레이캐스트에 사용하려면 비트연산으로 변환해야함

        MonsterData = monsterData;
        Speed = monsterData.speed;
        hp = monsterData.maxHp;

        // 몬스터가 최초로 스폰되거나 사망했었을 경우 -> 다시 초기화되면서 이동상태부터 시작해야함
        StateMachine.SetUpDefaultState();
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }

    public void TakeDamage(float damage)
    {
        var hitText = ObjectPoolManager.Instance.Get(EPool.TxtHitUI, new Vector2(transform.position.x, transform.position.y + 1f), Quaternion.identity);
        hitText.GetComponent<TxtHitUI>().InitializeTxtHitUI(damage);

        hp -= damage;
        if (hp <= 0)
            StateMachine.ExecuteCommand(EMonsterStateCommand.Die);

        TakeDamageEffect().Forget();
    }

    private void OnAttack()
    {
        // 공격 함수
    }

    private void OnDie()
    {
        ObjectPoolManager.Instance.Release(gameObject, EPool.Monster);
    }


    private async UniTaskVoid TakeDamageEffect()
    {
        try
        {
            // 피격시 0.1초 동안 흰색 머테리얼로 변경

            Material prevMaterial = sprites[0].material;
            ChangeMaterial(MonsterData.takeDamageMaterial);

            await UniTask.Delay(100);

            ChangeMaterial(prevMaterial);
        }
        catch (Exception ex)
        {
            Debug.Log($"TakeDamageEffect - {ex.Message}");
        }
    }

    private void ChangeMaterial(Material material)
    {
        foreach (var sprite in sprites)
        {
            sprite.material = material;
        }
    }
}
