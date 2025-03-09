using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCtrl : MonoBehaviour
{
    private Monster monster;
    private Rigidbody2D rigid;
    private bool isJump;

    public Rigidbody2D Rigid => rigid;
    public bool IsJump => isJump;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        monster = GetComponent<Monster>();
    }

    void FixedUpdate()
    {
            Debug.DrawRay(rigid.position, Vector3.down * 0.2f,Color.red);
        if (rigid.velocity.y <= 0) //내려갈떄만 스캔
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, 0.15f);
            if (hits.Length > 1)
            {
                isJump = false;
                return;
            }
        }
    }

    public void Jump()
    {
        if (isJump)
            return;

        isJump = true;
        rigid.AddForce(Settings.jumpDir * Settings.jumpForce, ForceMode2D.Impulse);
    }
}
