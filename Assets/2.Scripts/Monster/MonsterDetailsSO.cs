using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MonsterDetails_", menuName = "Scriptable Objects/Monster/Monster Details")]
public class MonsterDetailsSO : ScriptableObject
{
    [Header("Base Monster Details")]
    public string enemyName;
    public List<Sprite> sprites; // ������ ��������Ʈ

    [Header("Base Monster Ability")]
    public float speed;
    public float maxHp;
}
