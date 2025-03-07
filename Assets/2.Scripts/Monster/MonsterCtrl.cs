using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterCtrl : MonoBehaviour
{
    private Monster monster;
    private Rigidbody2D rigid;

    public Rigidbody2D Rigid => rigid;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        monster = GetComponent<Monster>();
    }

    public void Jump()
    {
        rigid.AddForce(Vector2.up * Settings.jumpForce, ForceMode2D.Impulse);
    }
}
