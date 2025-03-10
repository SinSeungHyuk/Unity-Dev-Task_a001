using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponAttackEvent : MonoBehaviour
{
    public event Action<WeaponAttackEvent, WeaponAttackEventArgs> OnWeaponAttack;

    public void CallWeaponAttackEvent(Weapon weapon)
    {
        OnWeaponAttack?.Invoke(this, new WeaponAttackEventArgs()
        {
            weapon = weapon,
        });
    }
}

public class WeaponAttackEventArgs : EventArgs
{
    public Weapon weapon;
}