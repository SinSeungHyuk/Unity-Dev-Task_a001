using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Monster : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> sprites;
    private LayerMask layerMask;

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

    void FixedUpdate()
    {
        Debug.DrawRay(transform.position, Vector3.up, Color.red);

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

        MonsterData = monsterData;
        SetLayerRecursively(gameObject, lane); // ��,��,�ϴ� �� ���� ������ ���ο� �´� ���̾� ����
        layerMask = 1 << lane; // �Ű������� ������ ���� ���̾�. ����ĳ��Ʈ�� ����Ϸ��� ��Ʈ�������� ��ȯ�ؾ���
        Speed = monsterData.speed;

        // ���Ƿ� ZombieStateMachine�� �־��� => ������� monsterData�� ���� ������ ���¸ӽ��� �޾ƿ;���
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
        // ���� �Լ�
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.TryGetComponent<Truck>(out var truck))
    //    {
    //        // Ʈ���� �浹�� ���ݻ��·� ����
    //        StateMachine.ExecuteCommand(EMonsterStateCommand.Attack);
    //    }

    //    // ���� ���̾� ������Ʈ���� �ε������� (���� ���ο� �ִ� ����)
    //    if (collision.gameObject.layer == this.gameObject.layer)
    //    {
    //        if (CheckCollisionDir(collision.gameObject) == ECollisionDir.Top)
    //            StateMachine.ExecuteCommand(EMonsterStateCommand.MoveBackward);
    //    }

    //    //if (StateMachine.IsJumpState())
    //    //    StateMachine.ExecuteCommand(EMonsterStateCommand.Move); // �������϶� �浹�ϸ� ���� ���� 
    //}

    //private void OnCollisionStay2D(Collision2D collision)
    //{
    //    // �ٸ� ���¿��� Ʈ���� �浹�� �̹� �߾ ���ݻ��·� ���̵Ǿ��ϹǷ� Stay���� �˻�
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
