using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroWeaponTransform : MonoBehaviour
{


    public void RotateWeapon(Vector2 dir)
    {
        float angle = UtilitieHelper.GetAngleFromVector(dir);

        // 무기 이미지가 약간 위로 기울어져있음
        transform.eulerAngles = new Vector3(0f, 0f, angle - 30f);
    }
}
