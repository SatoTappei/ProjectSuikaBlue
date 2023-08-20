using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// 隣のセルに移動を行い、隣のセルに到着したら評価ステートに戻る
    /// </summary>
    public class WanderState : BaseState
    {
        IBlackBoardForState _blackBoard;
        Transform _actor;
        Stack<Vector3> _path;
        Vector3 _currentCellPos;
        Vector3 _nextCellPos;
        float _lerpProgress;
        float _speedModify = 1;

        bool OnNextCell => _actor.position == _nextCellPos;

        public WanderState(IBlackBoardForState blackBoard) : base(StateType.Wander)
        {
            _blackBoard = blackBoard;
            _actor = blackBoard.Transform;
            _path = new();
        }

        protected override void Enter()
        {
            _lerpProgress = 0;

            // 正常なら8方向囲まれた位置に存在することが無い
            TryPathfinding();
            TryStepNextCell();
        }

        protected override void Exit()
        {          
        }

        protected override void Stay()
        {
            // 次のセルの上に来た場合は評価ステートに戻る
            if (OnNextCell)
            {
                ToEvaluateState();
            }
            else
            {
                Move();
            }
        }

        bool TryPathfinding()
        {
            _path.Clear();

            // 周囲8マスのランダムなセルに移動する
            Vector3 pos = _blackBoard.Transform.position;
            Vector2Int index = FieldManager.Instance.WorldPosToGridIndex(pos);
            foreach (Vector2Int dir in Utility.EightDirections.OrderBy(_ => System.Guid.NewGuid()))
            {
                Vector2Int neighbourIndex = index + dir;
                if (FieldManager.Instance.TryGetPath(index, neighbourIndex, out _path))
                {
                    return true;
                }
            }

            return false;
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