using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    /// <summary>
    /// プレイヤー/敵どちらも同じHPバー形式のUIを使用しているので
    /// 体力に関する処理だけを抜き出した基底クラス。使い方は以下の通り
    /// 1.オーバーライドした場合はこのクラスのAwake/Startも呼ぶ。
    /// 2.ダメージを受けた際にIDamageable型で取得してDamageメソッドを呼ぶ。
    /// </summary>
    [RequireComponent(typeof(HpBarController))]
    public class ActorBase : MonoBehaviour, IDamageable
    {
        [Header("ダメージ1の弾に対する体力")]
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
        /// 体力を減らしてUIに反映後、ダメージを受けたコールバックを呼ぶ
        /// 体力が0以下なら追加で撃破されたコールバックが呼ばれる
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
