using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(HpBarController))]
public class Damageable : MonoBehaviour, IDamageable
{
    [SerializeField] HpBarController _hpBar;
    [Header("�_���[�W1�̒e�ɑ΂���̗�")]
    [SerializeField] int _maxHp;

    int _currentHp;

    // int:�_���[�W GameObject:�U�����Ă�������
    public event UnityAction<int, GameObject> OnDamaged;
    public event UnityAction<GameObject> OnDefeated;

    void Start()
    {
        ResetParams();
    }

    /// <summary>
    /// �C�ӂ̃^�C�~���O�ŌĂяo���đ̗͍ő�Ƀ��Z�b�g�\
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
