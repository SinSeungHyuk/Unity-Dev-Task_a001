using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] private WeaponDetailsSO startingWeapon;
    private Weapon heroWeapon;

    public WeaponAttackEvent WeaponAttackEvent { get; private set; }


    private void Awake()
    {
        WeaponAttackEvent = GetComponent<WeaponAttackEvent>();
        heroWeapon = GetComponentInChildren<Weapon>();
    }

    void Start()
    {
        heroWeapon.InitializeWeapon(startingWeapon);
        WeaponAttackEvent.CallWeaponAttackEvent(heroWeapon);
    }
}
