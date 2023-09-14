using System.Linq;
using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// 隣のセルに移動を行い、隣のセルに到着したら評価ステートに戻る
    /// </summary>
    public class WanderState : BaseState
    {
        readonly MoveModule _move;
        readonly FieldModule _field;
        bool _hasNextCell;

        public WanderState(DataContext context) : base(context, StateType.Wander)
        {
            _move = new(context);
            _field = new(context);
        }

        protected override void Enter()
        {
            _move.Reset();
            _field.SetActorOnCell();
            _hasNextCell = SetTargetCell();
        }

        protected override void Exit()
        {          
        }

        protected override void Stay()
        {
            if (!_hasNextCell) { ToEvaluateState(); return; }

            if (_move.OnNextCell)
            {
                // 次のセルに到着したタイミングで移動前のセルの情報を消す
                _field.DeleteActorOnCell(_move.CurrentCellPos);

                ToEvaluateState();
            }
            else
            {
                _move.Move();
            }
        }

        bool SetTargetCell()
        {
            _move.CurrentCellPos = Context.Transform.position;

            // 周囲8マスのランダムなセルに移動する
            Vector3 pos = Context.Transform.position;
            Vector2Int index = FieldManager.Instance.WorldPosToGridIndex(pos);
            foreach (Vector2Int dir in Utility.EightDirections.OrderBy(_ => System.Guid.NewGuid()))
            {
                if (!FieldManager.Instance.IsWithinGrid(index + dir)) continue;

                if (FieldManager.Instance.TryGetCell(index + dir, out Cell cell))
                {
                    if (!cell.IsWalkable) continue;
                    if (!cell.IsEmpty) continue;

                    // 経路のセルとキャラクターの高さが違うので水平に移動させるために高さを合わせる
                    _move.NextCellPos = cell.Pos;
                    _move.NextCellPos.y = Context.Transform.position.y;
                    // 移動先のセルを予約する
                    _field.SetActorOnCell(_move.NextCellPos);

                    _move.Modify();
                    _move.Look();

                    return true;
                }
            }
            
            return false;
        }
    }
}