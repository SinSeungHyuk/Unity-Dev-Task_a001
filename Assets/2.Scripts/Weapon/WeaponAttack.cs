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
        weaponCosAngle = Mathf.Cos(weapon.WeaponAngle * 0.5f * Mathf.Deg2Rad); // 부채꼴 검사하기 위한 cos값

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
         * 1. 원을 그려서 가장 가까운 몬스터를 찾기
         * 2. 그 몬스터를 향한 방향벡터가 무기 발사의 기준벡터
         * 3. 해당 벡터를 기준으로 무기의 발사각도와 비교하여 범위에 포함되어있는지 검사
         * 4. 범위에 포함된 몬스터들에게 데미지 주기
         */

        // 1. 원 범위 내의 모든 몬스터 찾기
        Collider2D[] monstersInCircle = FindMonsterInCircle(transform.position, Settings.weaponRadius, Settings.monsterLayer);
        // 2. 가장 가까운 몬스터 찾기
        FindNearestMonster(monstersInCircle);
        // 3. 부채꼴 범위 내의 몬스터 검사, 데미지 처리
        FindMonsterInAngle(monstersInCircle);
        // 4. 무기 회전시키기
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
            // 단순 거리비교이므로 계산량이 적은 sqrMagnitude 사용
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

            // 두 방향벡터의 내적한 결과가 Cos값보다 '크면' 부채꼴 범위에 들어와있는것 (cos함수는 각도가 클수록 값이 작아지므로)
            // Vector2.Angle() 함수는 내부적으로 Acos을 사용하여 연산량이 더 크다고함 (따라서 이 방법 선택)
            if (Vector2.Dot(dirToMonster, dirToTarget) >= weaponCosAngle)
            {
                // 부채꼴 범위 내에 있음
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
