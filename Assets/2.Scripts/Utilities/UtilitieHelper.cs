using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;



// �������� ���ٰ����� ���� ��� static Ŭ����
public static class UtilitieHelper
{
    // ���ͷκ��� ���� ���ϱ�  ===========================================================
    public static float GetAngleFromVector(Vector3 vector)
    {
        float radians = Mathf.Atan2(vector.y, vector.x); // Atan2(y,x) ���� ���ϱ�
        float degrees = radians * Mathf.Rad2Deg;         // ���� ��׸��� ��ȯ

        return degrees;
    }
}