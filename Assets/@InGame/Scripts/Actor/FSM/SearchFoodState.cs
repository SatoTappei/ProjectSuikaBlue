using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PSB.InGame
{
    public class SearchFoodState : BaseState
    {
        //const float DistanceThreshold = 0.005f;

        IBlackBoardForState _blackBoard;
        Stack<Vector3> _path = new();
        Vector3 _nextCellPos;
        // 食料のセルがあり、食料までの経路が存在するかどうかのフラグ
        bool _hasPath;

        public SearchFoodState(IBlackBoardForState blackBoard) : base(StateType.Evaluate)
        {
            _blackBoard = blackBoard;
        }

        protected override void Enter()
        {
            _hasPath = TryPathfinding();
            _path.TryPop(out _nextCellPos);
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            if (!_hasPath)
            {
                TryChangeState(_blackBoard.EvaluateState);
                return;
            }

            if (!StepNextCell())
            {
                TryChangeState(_blackBoard.EvaluateState);
                return;
            }

            Move();

            // TODO: 経路探索 -> 経路中のセルの更新 -> 移動 の流れが出来たのでテストして本当に動くか試す

            // 向かう
            // 食べる
            // 評価ステートに遷移

            Log();
        }

        bool TryPathfinding()
        {
            _path.Clear();

            // 食料のセルがあるか調べる
            if (FieldManager.Instance.TryGetResourceCells(ResourceType.Tree, out List<Cell> cellList))
            {
                // 食料のセルを近い順に経路探索
                Vector3 pos = _blackBoard.Transform.position;
                foreach(Cell food in cellList.OrderBy(c => Vector3.SqrMagnitude(c.Pos - pos)))
                {
                    if(FieldManager.Instance.TryGetPath(pos, food.Pos, out _path))
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// 次に向かうセルの更新
        /// </summary>
        /// <returns>セルに移動中/次のセルがある:true 次のセルが無い(目的地に到着した):false</returns>
        bool StepNextCell()
        {
            if (_blackBoard.Transform.position != _nextCellPos) return true;
            return _path.TryPop(out _nextCellPos);
        }

        void Move()
        {
            Vector3 from = _blackBoard.Transform.position;
            Vector3 to = _nextCellPos;
            _blackBoard.Transform.position = Vector3.Slerp(from, to, Time.deltaTime * _blackBoard.Speed);
        }
    }
}