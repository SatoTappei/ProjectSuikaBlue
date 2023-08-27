using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class GatherState : BaseState
    {
        enum Stage
        {
            Move,
            Attack,
        }

        IBlackBoardForState _blackBoard;
        Stack<Vector3> _path = new();
        Transform _actor;
        Transform _leader;
        Vector3 _currentCellPos;
        Vector3 _nextCellPos;
        Stage _stage;
        float _lerpProgress;
        float _speedModify = 1;
        // 攻撃間隔のタイマー
        float _attackTimer;

        bool HasPath => _path.Count > 0;
        bool OnNextCell => _actor.position == _nextCellPos;

        public GatherState(IBlackBoardForState blackBoard) : base(StateType.Gather)
        {
            _blackBoard = blackBoard;
            _actor = _blackBoard.Transform;
        }

        protected override void Enter()
        {
            //_enemy = _blackBoard.Enemy;
            //_stage = Stage.Move;
            //_attackTimer = 0;
            _leader = _blackBoard.Leader.transform;
            TryPathfinding();
            TryStepNextCell();
            Debug.Log("集合!");
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            if (OnNextCell)
            {
                if (TryStepNextCell())
                {
                    // 経路の途中のセルの場合の処理
                }
                else
                {
                    ToEvaluateState();
                }
            }
            else
            {
                Move();
            }
        }

        bool TryPathfinding()
        {
            _path.Clear();
            return FieldManager.Instance.TryGetPath(_actor.position, _leader.transform.position, out _path);
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
