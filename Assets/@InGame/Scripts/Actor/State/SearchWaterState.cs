using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PSB.InGame
{
    // 食料のほぼコピペ

    /// <summary>
    /// 水のセルまで移動し、設定された効果値だけステータスの水の値を徐々に回復する。
    /// ステータスのパラメータが効果値を上回っていても、効果値分の回復処理が実行される
    /// </summary>
    public class SearchWaterState : BaseState
    {
        const int EffectValue = 100; // このステートで回復する水の値
        const int EffectDelta = 100; // 実際にはDeltaTimeとの乗算で回復する

        enum Stage
        {
            Move,
            Drink,
        }

        Stage _stage;
        Transform _actor;
        Stack<Vector3> _path = new();
        Vector3 _currentCellPos;
        Vector3 _nextCellPos;
        float _lerpProgress;
        float _effectProgress;
        float _speedModify = 1;
        // 食料のセルがあり、食料までの経路が存在するかどうかのフラグ
        bool _hasPath;

        bool OnNextCell => _actor.position == _nextCellPos;

        public SearchWaterState(DataContext context) : base(context, StateType.SearchWarter)
        {
            _actor = _blackBoard.Transform;
        }

        protected override void Enter()
        {
            _stage = Stage.Move;

            _effectProgress = 0;

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
                case Stage.Drink: DrinkStage(); break;
            }
        }

        bool TryPathfinding()
        {
            _path.Clear();

            // 食料のセルがあるか調べる
            if (FieldManager.Instance.TryGetResourceCells(ResourceType.Water, out List<Cell> cellList))
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
                    // 経路の途中のセルの場合の処理
                }
                else
                {
                    _stage = Stage.Drink; // 飲む状態へ
                }
            }
            else
            {
                Move();
            }
        }

        /// <summary>
        /// 水を飲む
        /// </summary>
        void DrinkStage()
        {
            if (!StepEatProgress()) { ToEvaluateState(); return; }
        }

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

        void Look()
        {
            Vector3 dir = _nextCellPos - _currentCellPos;
            _blackBoard.Model.rotation = Quaternion.LookRotation(dir, Vector3.up);
        }

        void Move()
        {
            _lerpProgress += Time.deltaTime * _blackBoard.Speed * _speedModify;
            _actor.position = Vector3.Lerp(_currentCellPos, _nextCellPos, _lerpProgress);
        }

        /// <summary>
        /// 回復の進捗度を進める
        /// </summary>
        /// <returns>回復の進捗中:true 回復の進捗が回復値に達した:false</returns>
        bool StepEatProgress()
        {
            float value = Time.deltaTime * EffectDelta;
            _effectProgress += value;
            _blackBoard.OnDrinkWaterInvoke(value); // 値の更新

            return _effectProgress <= EffectValue;
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