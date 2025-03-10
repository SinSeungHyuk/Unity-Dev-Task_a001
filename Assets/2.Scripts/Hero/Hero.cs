using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] private float radius = 5f;        // ��ä�� �ִ� �Ÿ�
    [SerializeField] private float coneAngle = 60f;    // ��ä�� ��ü ����
    [SerializeField] private LayerMask targetLayerMask;


    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);

        // ��ä�� ǥ�� (�ܼ� �ð�ȭ)
        Vector3 leftDir
            = Quaternion.Euler(0, 0, -coneAngle * 0.5f) * transform.right * radius;
        Vector3 rightDir
            = Quaternion.Euler(0, 0, coneAngle * 0.5f) * transform.right * radius;

        Gizmos.DrawLine(transform.position, transform.position + leftDir);
        Gizmos.DrawLine(transform.position, transform.position + rightDir);
    }
}
