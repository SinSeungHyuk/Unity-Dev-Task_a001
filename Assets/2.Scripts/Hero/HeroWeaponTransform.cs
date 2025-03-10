using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroWeaponTransform : MonoBehaviour
{


    public void RotateWeapon(Vector2 dir)
    {
        float angle = UtilitieHelper.GetAngleFromVector(dir);

        // ���� �̹����� �ణ ���� ����������
        transform.eulerAngles = new Vector3(0f, 0f, angle - 30f);
    }
}
