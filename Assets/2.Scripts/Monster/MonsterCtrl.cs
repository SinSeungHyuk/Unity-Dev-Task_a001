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
        //Landing Ploatform
        if (rigid.velocity.y < 0) //내려갈떄만 스캔
        {
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 0.1f);
            if (rayHit.collider != null)
            {
                isJump = false;
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
