using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public HeroWeaponTransform WeaponTransform {  get; private set; }
    public float WeaponDamage { get; private set; } // 데미지
    public float WeaponAngle { get; private set; } // 사격의 부채꼴 각도
    public float WeaponFireRate { get; private set; } // 사격의 부채꼴 각도


    private void Awake()
    {
        WeaponTransform = GetComponent<HeroWeaponTransform>();
    }

    public void InitializeWeapon(WeaponDetailsSO weaponDetails) // 무기 초기화
    {
        WeaponDamage = weaponDetails.weaponDamage;
        WeaponAngle = weaponDetails.weaponAngle;
        WeaponFireRate = weaponDetails.weaponFireRate;
    }
    
}
