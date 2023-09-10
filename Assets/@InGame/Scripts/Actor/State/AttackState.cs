using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UniRx;

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
        Stage _stage;
        // 周囲八近傍のセルの分だけ判定するので 8 で固定
        Collider[] _detected = new Collider[8];
        bool _firstStep; // 経路のスタート地点から次のセルに移動中

        public AttackState(DataContext context) : base(context, StateType.Attack)
        {
            _move = new(context);
            _field = new(context);
        }

        protected override void Enter()
        {
            TryStepNextCell();
            _field.SetActorOnCell();
            _stage = Stage.Move;
            _firstStep = true;

            // びっくりマーク再生
            Context.PlayBikkuri();
        }

        protected override void Exit()
        {
            // 使い終わった経路を消す
            Context.Path.Clear();
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
                        _field.DeleteActorOnCell(_move.CurrentCellPos);
                    }

                    // 別のステートが選択されていた場合は遷移する
                    if (Context.ShouldChangeState(this)) { ToEvaluateState(); return; }

                    // 現在のセルに他のキャラクターがいないかつ、周囲八近傍に敵がいた場合は攻撃
                    if (IsCellEmpty() && TryAttackNeighbour()) _stage = Stage.Attack;
                    else
                    {
                        if (!TryStepNextCell()) { ToEvaluateState(); return; }
                    }
                }
                else
                {
                    _move.Move();
                }
            }
            // 攻撃
            else if (_stage == Stage.Attack)
            {
                { ToEvaluateState(); return; }
            }
            // 1セル移動 -> 周囲八近傍に敵がいるか -> 居たら攻撃居なかったら次のセルへ

            // 近接攻撃どうする？
            //  敵に向かって歩いていく
            //  その場で敵が来たら攻撃

            // 攻撃は黒髪、金髪、金髪リーダー全部同じ
            // 経路を取得
            // 資源に向かうと違うのは資源と違い相手は動く
            //  つまり、経路探索を繰り返す必要がある。

            // 1対多の状況はどうする？
            // 敵を倒した際にも評価ステートに遷移する必要がある。
            //  案1:1発殴ったら評価ステートに遷移 <- 体力が減ったら自動で逃げるはず。
            //  殺した場合は敵を検知せずこのステートに遷移してこないはず。
            //  必要な値:総合スコア(色、サイズ) <- この値によって攻撃力が変わる.

            // 虚無に向かって攻撃しないか <- 死んだ場合はコライダーとレンダラが消えるので大丈夫
            // キャラクターのプーリングしたい
        }

        /// <summary>
        /// 各値を既定値に戻すことで、現在のセルの位置を自身の位置で更新する。
        /// 次のセルの位置をあれば次のセルの位置、なければ自身の位置で更新する。
        /// </summary>
        /// <returns>次のセルがある:true 次のセルが無い(目的地に到着):false</returns>
        bool TryStepNextCell()
        {
            _move.Reset();

            if (Context.Path.Count > 0)
            {
                // 経路の先頭(次のセル)から1つ取り出す
                _move.NextCellPos = Context.Path[0];
                Context.Path.RemoveAt(0);
                // 経路のセルとキャラクターの高さが違うので水平に移動させるために高さを合わせる
                _move.NextCellPos.y = Context.Transform.position.y;

                _move.Modify();
                _move.Look();
                return true;
            }
            else
            {
                _move.NextCellPos = Context.Transform.position;
                return false;
            }
        }

        /// <summary>
        /// 現在のセルに他のキャラクターがいるかどうかをレイキャストで判定する
        /// 経路の途中のセルは予約されていないのでレイキャストを用いる
        /// </summary>
        /// <returns>自分しかいない:true 誰かいる:false</returns>
        bool IsCellEmpty()
        {
            Vector3 pos = Context.Transform.position;
            float radius = 0.5f; // Scaleが1の場合の1セルの半径
            LayerMask layer = Context.Base.SightTargetLayer;
            Physics.OverlapSphereNonAlloc(pos, radius, _detected, layer);

            // 配列の中身でコンポーネントを取得出来た数が1の場合は自分しかいない
            return _detected.Where(c => c != null && c.TryGetComponent(out DataContext _)).Count() == 1;
        }

        /// <summary>
        /// 周囲八近傍の敵に対して攻撃を試みる
        /// </summary>
        /// <returns>攻撃成功:true 敵がいない:false</returns>
        bool TryAttackNeighbour()
        {
            Array.Clear(_detected, 0, _detected.Length);

            Vector3 pos = Context.Transform.position;
            LayerMask layer = Context.Base.SightTargetLayer;
            int count = Physics.OverlapSphereNonAlloc(pos, Utility.NeighbourCellRadius, _detected, layer);
            if (count == 0) return false;

            foreach (Collider collider in _detected)
            {
                if (collider == null) break;
                // 周囲八近傍に敵がいる場合は攻撃
                if (collider.CompareTag(Context.EnemyTag)) Attack();

                return true;
            }

            return false;
        }

        void Attack()
        {
            Debug.Log("攻撃！");
        }
    }
}
