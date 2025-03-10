using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class WeaponAttack : MonoBehaviour
{
    private WeaponAttackEvent weaponAttackEvent;
    private Weapon weapon;

    private Vector2 dirToMonster;
    private float weaponCosAngle;


    private void Awake()
    {
        weaponAttackEvent = GetComponent<WeaponAttackEvent>();
    }
    private void OnEnable()
    {
        weaponAttackEvent.OnWeaponAttack += WeaponAttackEvent_OnWeaponAttack;
    }
    private void OnDisable()
    {
        weaponAttackEvent.OnWeaponAttack -= WeaponAttackEvent_OnWeaponAttack;
    }



    private void WeaponAttackEvent_OnWeaponAttack(WeaponAttackEvent arg1, WeaponAttackEventArgs args)
    {
        weapon = args.weapon;
        weaponCosAngle = Mathf.Cos(weapon.WeaponAngle * 0.5f * Mathf.Deg2Rad); // ��ä�� �˻��ϱ� ���� cos��

        FireWeaponRoutine().Forget();
    }

    private async UniTask FireWeaponRoutine()
    {
        while (true)
        {
            float elapsedTime = 0f;
            float targetTime = weapon.WeaponFireRate;

            while (elapsedTime < targetTime)
            {
                elapsedTime += Time.deltaTime;

                await UniTask.Yield();
            }

            FireWeapon();
        }
    }

    private void FireWeapon()
    {
         /*
         * 1. ���� �׷��� ���� ����� ���͸� ã��
         * 2. �� ���͸� ���� ���⺤�Ͱ� ���� �߻��� ���غ���
         * 3. �ش� ���͸� �������� ������ �߻簢���� ���Ͽ� ������ ���ԵǾ��ִ��� �˻�
         * 4. ������ ���Ե� ���͵鿡�� ������ �ֱ�
         */

        // 1. �� ���� ���� ��� ���� ã��
        Collider2D[] monstersInCircle = FindMonsterInCircle(transform.position, Settings.weaponRadius, Settings.monsterLayer);
        // 2. ���� ����� ���� ã��
        FindNearestMonster(monstersInCircle);
        // 3. ��ä�� ���� ���� ���� �˻�, ������ ó��
        FindMonsterInAngle(monstersInCircle);
        // 4. ���� ȸ����Ű��
        RotateWeapon();
    }

    private Collider2D[] FindMonsterInCircle(Vector2 point, float radius, LayerMask monsterLayer)
        => Physics2D.OverlapCircleAll(point, radius, monsterLayer);

    private void FindNearestMonster(Collider2D[] monsters)
    {
        dirToMonster = Vector2.zero;
        float dist = Mathf.Infinity;

        for (int i = 0; i < monsters.Length; i++)
        {
            // �ܼ� �Ÿ����̹Ƿ� ��귮�� ���� sqrMagnitude ���
            float distance = (monsters[i].transform.position - transform.position).sqrMagnitude;
            if (dist > distance)
            {
                dist = distance;
                dirToMonster = (monsters[i].transform.position - transform.position).normalized;
            }
        }
    }

    private void FindMonsterInAngle(Collider2D[] monsters)
    {
        for (int i = 0; i < monsters.Length; i++)
        {
            Vector2 dirToTarget = (monsters[i].transform.position - transform.position).normalized;

            // �� ���⺤���� ������ ����� Cos������ 'ũ��' ��ä�� ������ �����ִ°� (cos�Լ��� ������ Ŭ���� ���� �۾����Ƿ�)
            // Vector2.Angle() �Լ��� ���������� Acos�� ����Ͽ� ���귮�� �� ũ�ٰ��� (���� �� ��� ����)
            if (Vector2.Dot(dirToMonster, dirToTarget) >= weaponCosAngle)
            {
                // ��ä�� ���� ���� ����
                monsters[i].GetComponent<Monster>().TakeDamage(weapon.WeaponDamage);
            }
        }
    }

    private void RotateWeapon()
    {
        if (dirToMonster == Vector2.zero)
            return;

        weapon.WeaponTransform.RotateWeapon(dirToMonster);
    }
}
