using System.Linq;
using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// 隣のセルに移動を行い、隣のセルに到着したら評価ステートに戻る
    /// </summary>
    public class WanderState : BaseState
    {
        Transform _actor;
        Vector3 _currentCellPos;
        Vector3 _nextCellPos;
        float _lerpProgress;
        float _speedModify = 1;

        bool OnNextCell => _actor.position == _nextCellPos;

        public WanderState(DataContext context) : base(context, StateType.Wander)
        {
            _actor = context.Transform;
        }

        protected override void Enter()
        {
            _lerpProgress = 0;

            // 正常なら8方向囲まれた位置に存在することが無い
            SetTargetCell();
            Modify();
            Look();
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

        bool SetTargetCell()
        {
            _currentCellPos = _actor.position;

            // 周囲8マスのランダムなセルに移動する
            Vector3 pos = Context.Transform.position;
            Vector2Int index = FieldManager.Instance.WorldPosToGridIndex(pos);
            foreach (Vector2Int dir in Utility.EightDirections.OrderBy(_ => System.Guid.NewGuid()))
            {
                if (FieldManager.Instance.TryGetCell(index + dir, out Cell cell))
                {
                    if (!cell.IsWalkable) continue;

                    // 経路のセルとキャラクターの高さが違うので水平に移動させるために高さを合わせる
                    _nextCellPos = cell.Pos;
                    _nextCellPos.y = _actor.position.y;
                    return true;
                }
            }
            
            return false;
        }

        void ToEvaluateState() => TryChangeState(Context.EvaluateState);

        void Move()
        {
            //_lerpProgress += Time.deltaTime * Context.Speed * _speedModify;
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

        void Look()
        {
            Vector3 dir = _nextCellPos - _currentCellPos;
            Context.Model.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }
    }
}