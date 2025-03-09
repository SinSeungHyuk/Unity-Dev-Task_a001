using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Truck : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    private Rigidbody2D rigid;
    private Vector2 dir;

    private float collisionCount = 0;
    private float currentSpeed;
    private Vector2 currentPos;

    public float Speed => currentSpeed;


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        dir = Vector2.right;
        currentSpeed = speed;
    }

    private void FixedUpdate()
    {
        rigid.velocity = dir * currentSpeed;
    }

    //public void SetSpeed(bool isCollisionEnter)
    //{
    //    collisionCount = (isCollisionEnter) ? collisionCount+1 : collisionCount-1;

    //    if (collisionCount >= Settings.truckStopCount)
    //    {
    //        currentSpeed = 0f;
    //        return;
    //    }

    //    currentSpeed = speed * (1 - (float)collisionCount * 0.3f);
    //}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Monster>(out var monster))
        {

            collisionCount = Mathf.Clamp(collisionCount + 1, 0, Settings.truckStopCount);

            SetSpeed();
        }
    }


    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Monster>(out var monster))
        {

            collisionCount = Mathf.Clamp(collisionCount - 1, 0, Settings.truckStopCount);

            SetSpeed();
        }
    }

    private void SetSpeed()
    {
        currentSpeed = speed * (1f - (float)(collisionCount / Settings.truckStopCount));
    }
}
