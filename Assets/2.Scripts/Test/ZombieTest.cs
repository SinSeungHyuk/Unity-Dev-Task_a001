using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieTest : MonoBehaviour
{
    public float speed = 3f;
    public float jumpForce = 7f;
    public float gravity = 20f;
    public float jumpSpeed = 2f;

    private bool isJumping = false;
    private float yPos;
    private Vector2 dir;

    private Rigidbody2D rigid;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        dir = new Vector2(-1f, 0f);
    }

    private void Start()
    {
        yPos = transform.position.y;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // ���� ���� �ƴ϶�� ���� y���� ����
        if (!isJumping)
        {
            transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
        }
        else
        {
            // ���� ���̶�� �߷� ����
            dir.y -= gravity * Time.deltaTime;
            dir.Normalize();

            // ���� �� �������� ���� ���� ��ġ���� �������� ���� ����
            if (dir.y < 0 && transform.position.y <= yPos)
            {
                isJumping = false;
                dir.y = 0;
                dir.Normalize();
                transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
            }
        }
    }

    private void FixedUpdate()
    {
        rigid.velocity = dir * speed;
    }

    private void Jump()
    {
        if (!isJumping)
        {
            isJumping = true;

            rigid.AddForce(Vector3.up * jumpForce);

            //dir.y = jumpForce;

            //dir.Normalize();
        }
    }

    private void OnAttack()
    {
        // ���� �Լ�
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("MidLane"))
        {

        }
    }
}
