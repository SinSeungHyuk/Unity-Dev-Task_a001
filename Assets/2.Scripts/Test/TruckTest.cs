using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TruckTest : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Rigidbody2D rigid;
    private Vector2 dir;

    public float Speed => speed;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        dir = new Vector2(1f, 0f);
    }

    private void FixedUpdate()
    {
        rigid.velocity = dir * speed;
    }
}
