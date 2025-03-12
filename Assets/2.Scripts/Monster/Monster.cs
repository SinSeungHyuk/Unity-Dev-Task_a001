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
    private float hp; // �ӽ÷� �̰��� hp �������

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
        // ���Ƿ� ZombieStateMachine�� �־��� => ������� monsterData�� ���� ������ ���¸ӽ��� �޾ƿ;���
        StateMachine = GetComponent<ZombieStateMachine>();
        StateMachine.InitializeStateMachine(this);
    }

    void FixedUpdate()
    {
        // �������϶��� �޹��� ���·� �����ϹǷ� ����ȭ�� ���� ������� �˻�
        if (StateMachine.IsAttackState() == true)
        {
            RaycastHit2D[] upHits = Physics2D.RaycastAll(transform.position, Vector2.up, 1f, layerMask);
            foreach (var hit in upHits)
            {
                if (hit.collider.gameObject == this.gameObject) continue;

                StateMachine.ExecuteCommand(EMonsterStateCommand.MoveBackward);
            }
        }

        // �̵����϶��� ����or���� ���·� �����ϹǷ� ����ȭ�� ���� ������� �˻�
        if (StateMachine.IsMoveToTruckState() == true)
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.left, 1f);
            foreach (var hit in hits)
            {
                if (hit.collider.gameObject == this.gameObject) continue;

                if (hit.collider.gameObject.layer == this.gameObject.layer && !StateMachine.IsJumpState()) // ���� ������ ����
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

        SetLayerRecursively(gameObject, lane); // ��,��,�ϴ� �� ���� ������ ���ο� �´� ���̾� ����
        layerMask = 1 << lane; // �Ű������� ������ ���� ���̾�. ����ĳ��Ʈ�� ����Ϸ��� ��Ʈ�������� ��ȯ�ؾ���

        MonsterData = monsterData;
        Speed = monsterData.speed;
        hp = monsterData.maxHp;

        // ���Ͱ� ���ʷ� �����ǰų� ����߾��� ��� -> �ٽ� �ʱ�ȭ�Ǹ鼭 �̵����º��� �����ؾ���
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
        // ���� �Լ�
    }

    private void OnDie()
    {
        ObjectPoolManager.Instance.Release(gameObject, EPool.Monster);
    }


    private async UniTaskVoid TakeDamageEffect()
    {
        try
        {
            // �ǰݽ� 0.1�� ���� ��� ���׸���� ����

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
