using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    /// <summary>
    /// �v���C���[/�G�ǂ��������HP�o�[�`����UI���g�p���Ă���̂�
    /// �̗͂Ɋւ��鏈�������𔲂��o�������N���X�B�g�����͈ȉ��̒ʂ�
    /// 1.�I�[�o�[���C�h�����ꍇ�͂��̃N���X��Awake/Start���ĂԁB
    /// 2.�_���[�W���󂯂��ۂ�IDamageable�^�Ŏ擾����Damage���\�b�h���ĂԁB
    /// </summary>
    [RequireComponent(typeof(HpBarController))]
    public class ActorBase : MonoBehaviour, IDamageable
    {
        [Header("�_���[�W1�̒e�ɑ΂���̗�")]
        [SerializeField] int _maxHp;

        HpBarController _hpBar;
        int _currentHp;

        protected int MaxHp => _maxHp;

        protected virtual void Awake()
        {
            _hpBar = GetComponent<HpBarController>();
            _currentHp = MaxHp;
        }

        protected virtual void Start()
        {
            _hpBar.Draw(MaxHp, MaxHp);
        }

        /// <summary>
        /// �̗͂����炵��UI�ɔ��f��A�_���[�W���󂯂��R�[���o�b�N���Ă�
        /// �̗͂�0�ȉ��Ȃ�ǉ��Ō��j���ꂽ�R�[���o�b�N���Ă΂��
        /// </summary>
        void IDamageable.Damage(int value)
        {
            _currentHp -= value;
            _hpBar.Draw(_currentHp, MaxHp);
            Damage(value);

            if (_currentHp <= 0) Defeated();
        }

        protected virtual void Damage(int value) { }
        protected virtual void Defeated() { }
    }
}
