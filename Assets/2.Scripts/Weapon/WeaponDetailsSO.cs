using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponDetails_", menuName = "Scriptable Objects/Weapon")]
public class WeaponDetailsSO : ScriptableObject
{
    [Header("Weapon Base Details")]
    public string weaponName;

    [Header("Weapon Ability")]
    public float weaponDamage;
    public float weaponAngle; // 사격의 부채꼴 각도
    public float weaponFireRate; // 사격속도
}
