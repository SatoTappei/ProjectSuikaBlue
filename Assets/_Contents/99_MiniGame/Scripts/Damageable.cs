using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(HpBarController))]
public class Damageable : MonoBehaviour, IDamageable
{
    [SerializeField] HpBarController _hpBar;
    [Header("ダメージ1の弾に対する体力")]
    [SerializeField] int _maxHp;

    int _currentHp;

    // int:ダメージ GameObject:攻撃してきた相手
    public event UnityAction<int, GameObject> OnDamaged;
    public event UnityAction<GameObject> OnDefeated;

    void Start()
    {
        ResetParams();
    }

    /// <summary>
    /// 任意のタイミングで呼び出して体力最大にリセット可能
    /// </summary>
    public void ResetParams()
    {
        _currentHp = _maxHp;
        _hpBar.Draw(_maxHp, _maxHp);
    }

    void IDamageable.Damage(int value, GameObject attacker)
    {
        _currentHp -= value;
        _hpBar.Draw(_currentHp, _maxHp);
        OnDamaged?.Invoke(value, attacker);

        if (_currentHp <= 0) OnDefeated?.Invoke(attacker);
    }
}
