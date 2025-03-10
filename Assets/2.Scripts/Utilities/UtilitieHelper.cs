using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



// 전역으로 접근가능한 각종 계산 static 클래스
public static class UtilitieHelper
{
    // 벡터로부터 각도 구하기  ===========================================================
    public static float GetAngleFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.y, vector.x); // Atan2(y,x) 라디안 구하기
        float degrees = radians * Mathf.Rad2Deg;         // 라디안 디그리로 변환

        return degrees;
    }
}