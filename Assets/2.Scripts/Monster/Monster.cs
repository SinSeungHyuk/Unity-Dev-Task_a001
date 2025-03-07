using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [SerializeField] private List<SpriteRenderer> sprites;

    private BoxCollider2D hitBox;

    public BoxCollider2D HitBox => hitBox;
    public MonsterCtrl MonsterCtrl {  get; private set; }
    public ZombieStateMachine StateMachine { get; private set; }
    public Animator Animator { get; private set; }
    public float Speed {  get; private set; }



    private void Awake()
    {
        MonsterCtrl = GetComponent<MonsterCtrl>();
        hitBox = GetComponent<BoxCollider2D>();
        Animator = GetComponent<Animator>();
    }

    public void InitializeMonster(MonsterDetailsSO monsterData, LayerMask lane)
    {
        sprites.Zip(monsterData.sprites, (sprite, dataSprite) => sprite.sprite = dataSprite).ToList();

        this.gameObject.layer = lane;
        Speed = monsterData.speed;

        // ���Ƿ� ZombieStateMachine�� �־��� => ������� monsterData�� ���� ������ ���¸ӽ��� �޾ƿ;���
        StateMachine = GetComponent<ZombieStateMachine>();
        StateMachine.InitializeStateMachine(this);
    }

    private void OnAttack()
    {
        // ���� �Լ�
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {



        if (collision.gameObject.TryGetComponent<Truck>(out var truck))
        {
            StateMachine.ExecuteCommand(EMonsterStateCommand.Attack);
            truck.SetSpeed(true);
        }

        // ���� ���̾� ������Ʈ���� �ε������� (���� ���ο� �ִ� ����)
        if (collision.gameObject.layer == this.gameObject.layer)
        {
            Vector2 contactNormal = collision.GetContact(0).normal;

            float absX = Mathf.Abs(contactNormal.x);
            float absY = Mathf.Abs(contactNormal.y);

            if (absX > absY)
            {
                // X�� ���� �浹 (��-��)
                if (contactNormal.x > 0)
                    StateMachine.ExecuteCommand(EMonsterStateCommand.Jump);
            }

            else if (absY > absX)
            {
                // Y�� ���� �浹 (��-��)
                if (contactNormal.y < 0)
                    StateMachine.ExecuteCommand(EMonsterStateCommand.MoveBackward);
            }
        }


    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Truck>(out var truck))
        {
            truck.SetSpeed(false);
            Animator.SetBool("IsAttacking", false);
        }
    }
}
