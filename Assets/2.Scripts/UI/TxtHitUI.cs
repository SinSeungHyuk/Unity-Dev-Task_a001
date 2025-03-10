using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using UnityEngine;

public class TxtHitUI : MonoBehaviour
{
    private TextMeshPro txtDamage;


    private void Awake()
    {
        txtDamage = GetComponent<TextMeshPro>();
    }

    public void InitializeTxtHitUI(float damageAmount)
    {
        txtDamage.text = damageAmount.ToString("F0");

        transform.DOMoveY(0.5f, 0.3f).SetEase(Ease.InOutQuad).SetRelative()
            .OnComplete(() => ObjectPoolManager.Instance.Release(gameObject, EPool.TxtHitUI));
    }
}
