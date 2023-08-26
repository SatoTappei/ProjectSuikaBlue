using System.Collections.Generic;
using System.Linq;
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

        IBlackBoardForState _blackBoard;
        Stack<Vector3> _path = new();
        Transform _actor;
        Actor _enemy;
        Vector3 _currentCellPos;
        Vector3 _nextCellPos;
        Stage _stage;
        float _lerpProgress;
        float _speedModify = 1;
        // 攻撃間隔のタイマー
        float _attackTimer;

        bool HasPath => _path.Count > 0;
        bool OnNextCell => _actor.position == _nextCellPos;

        public AttackState(IBlackBoardForState blackBoard) : base(StateType.Attack)
        {
            _blackBoard = blackBoard;
            _actor = _blackBoard.Transform;
        }

        protected override void Enter()
        {
            _enemy = _blackBoard.Enemy;
            _stage = Stage.Move;
            _attackTimer = 0;
            TryPathfinding();
            TryStepNextCell();
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            // 近接攻撃どうする？
            //  敵に向かって歩いていく
            //  その場で敵が来たら攻撃

            // 攻撃は黒髪、金髪、金髪リーダー全部同じ
            // 経路を取得
            // 資源に向かうと違うのは資源と違い相手は動く
            //  つまり、経路探索を繰り返す必要がある。

            // 1対多の状況はどうする？

            // 経路が無いので評価ステートに遷移
            //if (!_hasPath) { ToEvaluateState(); return; }

            if (OnNextCell)
            {
                // 違うステートなら遷移する
                if(_blackBoard.NextState.Type != StateType.Attack)
                {
                    ToEvaluateState();
                }
            }

            switch (_stage)
            {
                case Stage.Move: MoveStage(); break;
                case Stage.Attack: AttackStage(); break;
            }
        }

        void MoveStage()
        {
            if (OnNextCell)
            {
                if (TryStepNextCell())
                {
                    // 経路の途中のセルの場合の処理
                }
                else
                {
                    _stage = Stage.Attack;
                    _attackTimer = 3.1f; // 攻撃間隔を超える適当な値、最初に一発殴る
                }
            }
            else
            {
                Move();
            }
        }

        void AttackStage()
        {
            _attackTimer += Time.deltaTime;
            if (_attackTimer > 3) // 適当な値
            {
                _attackTimer = 0;
                float d =  Vector3.Distance(_actor.position, _enemy.transform.position);

                // 1マスとなりなのでこのくらい
                if (d <= 1.5f)
                {
                    _enemy.Damaged();
                }
                else
                {
                    _stage = Stage.Move;

                    TryPathfinding();
                    TryStepNextCell();
                }
            }
        }

        void ToEvaluateState() => TryChangeState(_blackBoard.EvaluateState);

        /// <summary>
        /// 現在のセルの位置を自身の位置で更新する。
        /// 次のセルの位置をあれば次のセルの位置、なければ自身の位置で更新する。
        /// </summary>
        /// <returns>次のセルがある:true 次のセルが無い(目的地に到着):false</returns>
        bool TryStepNextCell()
        {
            _currentCellPos = _actor.position;

            if (_path.TryPop(out _nextCellPos))
            {
                // 経路のセルとキャラクターの高さが違うので水平に移動させるために高さを合わせる
                _nextCellPos.y = _actor.position.y;
                Modify();
                Look();
                _lerpProgress = 0;

                return true;
            }

            _nextCellPos = _actor.position;

            return false;
        }

        bool TryPathfinding()
        {
            _path.Clear();
            return FieldManager.Instance.TryGetPath(_actor.position, _enemy.transform.position, out _path);
        }

        void Move()
        {
            _lerpProgress += Time.deltaTime * _blackBoard.Speed * _speedModify;
            _actor.position = Vector3.Lerp(_currentCellPos, _nextCellPos, _lerpProgress);
        }

        void Look()
        {
            Vector3 dir = _nextCellPos - _currentCellPos;
            _blackBoard.Model.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }

        /// <summary>
        /// 斜め移動の速度を補正する
        /// </summary>
        void Modify()
        {
            bool dx = Mathf.Approximately(_currentCellPos.x, _nextCellPos.x);
            bool dz = Mathf.Approximately(_currentCellPos.z, _nextCellPos.z);

            _speedModify = (dx || dz) ? 1 : 0.7f;
        }
    }
}
