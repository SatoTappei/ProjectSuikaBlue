using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HpBarController : MonoBehaviour
{
    [SerializeField] Transform _hpBar;

    void Awake()
    {
        _hpBar.localScale = Vector3.one;
    }

    public void Draw(int currentHp, int maxHp)
    {
        float value = 1.0f * currentHp / maxHp;
        _hpBar.localScale = new Vector3(Mathf.Max(value, 0), 1, 1);
    }
}
