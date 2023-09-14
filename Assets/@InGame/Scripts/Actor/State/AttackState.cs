using System.Linq;
using UniRx;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class AttackState : BaseState
    {
        enum Stage
        {
            Move,
            Attack,
        }

        readonly MoveModule _move;
        readonly FieldModule _field;
        readonly DetectModule _detect;
        
        Stage _stage;
        bool _firstStep; // 経路のスタート地点から次のセルに移動中

        public AttackState(DataContext context) : base(context, StateType.Attack)
        {
            _move = new(context);
            _field = new(context);
            _detect = new(context);
        }

        Collider[] Detecetd => Context.Detected;
        List<Vector3> Path => Context.Path;
        string EnemyTag => Context.EnemyTag;
        int MeleeDamage => Context.Base.MeleeDamage;
        DataContext Enemy { set => Context.Enemy = value; }

        protected override void Enter()
        {
            _move.TryStepNextCell();
            _field.SetOnCell();
            _stage = Stage.Move;
            _firstStep = true;

            // びっくりマーク再生
            Context.PlayDiscoverEffect();
        }

        protected override void Exit()
        {
            Enemy = null;
            // 使い終わった経路を消す
            Path.Clear();
        }       

        protected override void Stay()
        {
            // 移動
            if (_stage == Stage.Move)
            {
                if (_move.OnNextCell)
                {
                    // 経路のスタート地点は予約されているので、次のセルに移動した際に消す
                    // 全てのセルに対して行うと、別のキャラクターで予約したセルまで消してしまう。
                    if (_firstStep)
                    {
                        _firstStep = false;
                        _field.DeleteOnCell(_move.CurrentCellPos);
                    }

                    // 別のステートが選択されていた場合は遷移する
                    if (Context.ShouldChangeState(this)) { ToEvaluateState(); return; }

                    // 現在のセルに他のキャラクターがいないかつ、周囲八近傍に敵がいた場合は攻撃
                    if (IsCellEmpty() && TryAttackNeighbour()) _stage = Stage.Attack;
                    // 経路の末端まで辿り着いた場合は遷移
                    else if (!_move.TryStepNextCell()) { ToEvaluateState(); return; }
                }
                else
                {
                    _move.Move();
                }
            }
            // 攻撃
            else if (_stage == Stage.Attack)
            {
                // Damageメソッドを呼び出すタイミングでパーティクルが再生されるので
                // アニメーションの再生等をしない場合、このまま評価ステートに遷移
                ToEvaluateState();
            }
        }

        /// <summary>
        /// 現在のセルに他のキャラクターがいるかどうかをレイキャストで判定する
        /// 経路の途中のセルは予約されていないのでレイキャストを用いる
        /// </summary>
        /// <returns>自分しかいない:true 誰かいる:false</returns>
        bool IsCellEmpty()
        {
            // 半径はScaleが1の場合の1セルの半径
            _detect.OverlapSphere(0.5f); 
            // 配列の中身でコンポーネントを取得出来た数が1の場合は自分しかいない
            return Detecetd.Where(c => c != null && c.TryGetComponent(out DataContext _)).Count() == 1;
        }

        /// <summary>
        /// 周囲八近傍の敵に対して攻撃を試みる
        /// </summary>
        /// <returns>攻撃成功:true 敵がいない:false</returns>
        bool TryAttackNeighbour()
        {
            _detect.OverlapSphere(Utility.NeighbourCellRadius);
            
            foreach (Collider collider in Detecetd)
            {
                if (collider == null) break;
                // 周囲八近傍に敵がいる場合は攻撃
                if (collider.CompareTag(EnemyTag) && TryAttack()) return true;
            }

            return false;
        }

        bool TryAttack()
        {
            if (Context.Enemy == null) return false;

            Context.Enemy.Damage(MeleeDamage);
            return true;
        }
    }
}
