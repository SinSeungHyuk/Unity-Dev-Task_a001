using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public HeroWeaponTransform WeaponTransform {  get; private set; }
    public float WeaponDamage { get; private set; } // ������
    public float WeaponAngle { get; private set; } // ����� ��ä�� ����
    public float WeaponFireRate { get; private set; } // ����� ��ä�� ����


    private void Awake()
    {
        WeaponTransform = GetComponent<HeroWeaponTransform>();
    }

    public void InitializeWeapon(WeaponDetailsSO weaponDetails) // ���� �ʱ�ȭ
    {
        WeaponDamage = weaponDetails.weaponDamage;
        WeaponAngle = weaponDetails.weaponAngle;
        WeaponFireRate = weaponDetails.weaponFireRate;
    }
    
}
