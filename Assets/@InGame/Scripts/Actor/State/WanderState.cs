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

        public WanderState(DataContext context) : base(context, StateType.Wander)
        {
            _move = new(context);
            _field = new(context);
        }

        Vector3 Position => Context.Transform.position;

        protected override void Enter()
        {
            _move.Reset();
            _field.SetOnCell(_move.CurrentCellPos);
            SetNeighbourCell();
        }

        protected override void Exit()
        {
            _field.DeleteOnCell(_move.CurrentCellPos);
            _field.DeleteOnCell(_move.NextCellPos);
        }

        protected override void Stay()
        {
            if (_move.OnNextCell)
            {
                ToEvaluateState();
            }
            else
            {
                _move.Move();
            }
        }

        void SetNeighbourCell()
        {
            // 周囲8マスのランダムなセルに移動する
            Vector2Int index = FieldManager.Instance.WorldPosToGridIndex(Position);
            foreach (Vector2Int dir in Utility.EightDirections.OrderBy(_ => System.Guid.NewGuid()))
            {
                // キャラクターがいないセルの取得
                if (!FieldManager.Instance.TryGetCell(index + dir, out Cell cell)) continue;
                if (!cell.IsEmpty) continue;

                // 経路のセルとキャラクターの高さが違うので水平に移動させるために高さを合わせる
                _move.NextCellPos = new Vector3(cell.Pos.x, Position.y, cell.Pos.z);
                // 移動先のセルを予約する
                _field.SetOnCell(_move.NextCellPos);

                _move.Modify();
                _move.Look();
                break;
            }
        }
    }
}