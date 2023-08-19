using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PSB.InGame
{
    public class SearchFoodState : BaseState
    {
        //const float DistanceThreshold = 0.005f;
        enum Stage
        {
            Move,
            Eat,
        }

        IBlackBoardForState _blackBoard;
        Transform _actor;
        Stack<Vector3> _path = new();
        Vector3 _currentCellPos;
        Vector3 _nextCellPos;
        float _lerpProgress = 0;
        // 食料のセルがあり、食料までの経路が存在するかどうかのフラグ
        bool _hasPath;
        Stage _stage;

        bool OnNextCell => _actor.position == _nextCellPos;

        public SearchFoodState(IBlackBoardForState blackBoard) : base(StateType.SearchFood)
        {
            _blackBoard = blackBoard;
            _actor = _blackBoard.Transform;
        }

        protected override void Enter()
        {
            _stage = Stage.Move;

            _hasPath = TryPathfinding();
            TryStepNextCell();
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            // 経路が無いので評価ステートに遷移
            if (!_hasPath) { ToEvaluateState(); return; }

            switch (_stage)
            {
                case Stage.Move: MoveStage(); break;
                case Stage.Eat:  EatStage();  break;
            }
        }

        bool TryPathfinding()
        {
            _path.Clear();

            // 食料のセルがあるか調べる
            if (FieldManager.Instance.TryGetResourceCells(ResourceType.Tree, out List<Cell> cellList))
            {
                // 食料のセルを近い順に経路探索
                Vector3 pos = _actor.position;
                foreach (Cell food in cellList.OrderBy(c => Vector3.SqrMagnitude(c.Pos - pos)))
                {
                    if (FieldManager.Instance.TryGetPath(pos, food.Pos, out _path)) // <- ｱﾔｼｲ
                    {
                        return true;
                    }
                }

                return false;
            }

            return false;
        }

        void ToEvaluateState() => TryChangeState(_blackBoard.EvaluateState);

        /// <summary>
        /// 食料のセルに移動
        /// </summary>
        void MoveStage()
        {
            // 次のセルの上に来た場合はチェックする
            if (OnNextCell)
            {
                // 違うステートに遷移する場合は一度評価ステートを経由する
                if (_blackBoard.NextState != this) { ToEvaluateState(); return; }

                if (TryStepNextCell())
                {
                    Debug.Log("次 " + _blackBoard.Transform.name);

                }
                else
                {
                    Debug.Log("目的地 " + _blackBoard.Transform.name);
                    _stage = Stage.Eat; // 食べる状態へ
                }
            }
            else
            {
                Move();
            }
        }

        /// <summary>
        /// 食料を食べる
        /// </summary>
        void EatStage()
        {
            _blackBoard.OnEatFoodInvoke(1);
            EatFood();

            // ステータスを変更する
            //  黒板にステータスへの参照を持たせる？ただし、ステータスはものびを継承していない。
            //  デリゲートの利用？
            //  メッセージングの場合は、クラス全体に送信されてしまうが、他の個体と連携が取れる？
            // 体力をどれくらい回復するのか？
            // 食料や水には回復量を設定し、その値だけ回復する
        }

        /// <summary>
        /// 現在のセルの位置を自身の位置で更新する。
        /// 次のセルの位置をあれば次のセルの位置、なければ自身の位置で更新する。
        /// </summary>
        /// <returns>次のセルがある:true 次のセルが無い(目的地に到着):false</returns>
        bool TryStepNextCell()
        {
            _currentCellPos = _actor.position;

            if(_path.TryPop(out _nextCellPos))
            {
                // 経路のセルとキャラクターの高さが違うので水平に移動させるために高さを合わせる
                _nextCellPos.y = _actor.position.y;
                _lerpProgress = 0;
                return true;
            }

            _nextCellPos = _actor.position;

            return false;
        }

        /// <summary>
        /// 次のセルを調べる。
        /// セルにたどり着いて、次のセルがある場合は更新する。
        /// </summary>
        /// <returns>セルに移動中/次のセルがある:true 次のセルが無い(目的地に到着した):false</returns>
        bool CheckNextCell()
        {
            if (_actor.position != _nextCellPos) return true;
            return TryStepNextCell();
        }

        void Move()
        {
            _lerpProgress += Time.deltaTime * _blackBoard.Speed;
            _actor.position = Vector3.Lerp(_currentCellPos, _nextCellPos, _lerpProgress);
        }

        void EatFood()
        {
            Debug.Log("もぐもぐ");
        }

        void DebugVisualize()
        {
            Stack<Vector3> temp = new(_path);
            foreach(Vector3 p in temp)
            {
                var g = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                g.transform.position = p + Vector3.up;
            }
        }
    }
}