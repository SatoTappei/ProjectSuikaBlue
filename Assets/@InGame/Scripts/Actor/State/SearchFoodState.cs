using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// 水のセルまで移動し、設定された効果値だけステータスの水の値を徐々に回復する。
    /// ステータスのパラメータが効果値を上回っていても、効果値分の回復処理が実行される
    /// </summary>
    public class SearchFoodState : BaseState
    {
        const int EffectValue = 100; // このステートで回復する食料の値
        const int EffectDelta = 100; // 実際にはDeltaTimeとの乗算で回復する

        enum Stage
        {
            Move,
            Eat,
        }

        MoveModule _move;
        FieldModule _field;

        Stage _stage;
        List<Vector3> _path = new();
        float _effectProgress;
        bool _hasPath;

        public SearchFoodState(DataContext context) : base(context, StateType.SearchFood)
        {
            _move = new(context);
            _field = new(context);
        }

        protected override void Enter()
        {
            _move.Reset();
            _stage = Stage.Move;
            _effectProgress = 0;

            _field.SetActorOnCell();

            _hasPath = TryPathfinding();
            TryStepNextCell();
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            if (!_hasPath) { ToEvaluateState(); return; }

            // 移動
            if (_stage == Stage.Move)
            {
                if (_move.OnNextCell)
                {
                    // 次のセルに到着したタイミングで移動前のセルの情報を消す
                    _field.DeleteActorOnCell(_move.CurrentCellPos);

                    // 別のステートが選択されていた場合は遷移する
                    if (Context.ShouldChangeState(this)) { ToEvaluateState(); return; }

                    if (TryStepNextCell())
                    {
                        // 経路の途中のセルの場合の処理
                    }
                    else
                    {
                        _stage = Stage.Eat; // 食べる状態へ
                    }
                }
                else
                {
                    _move.Move();
                }
            }
            // 食べる
            else
            {
                Debug.Log("ﾓｸﾞﾓｸﾞ");
                // 食べる時はエフェクトが欲しい。
                //if (!StepEatProgress()) { ToEvaluateState(); return; }
                Context.Food.Value += 100;
                ToEvaluateState();
            }
        }

        bool TryPathfinding()
        {
            _path.Clear();

            // 食料のセルがあるか調べる
            if (FieldManager.Instance.TryGetResourceCells(ResourceType.Tree, out List<Cell> cellList))
            {
                // 食料のセルを近い順に経路探索
                Vector3 pos = Context.Transform.position;
                foreach (Cell food in cellList.OrderBy(c => Vector3.SqrMagnitude(c.Pos - pos)))
                {
                    // TODO:全ての食料に対して経路探索をすると重いのである程度の所で打ち切る処理

                    Vector2Int currentIndex = FieldManager.Instance.WorldPosToGridIndex(pos);
                    Vector2Int foodIndex = FieldManager.Instance.WorldPosToGridIndex(food.Pos);

                    int dx = Mathf.Abs(currentIndex.x - foodIndex.x);
                    int dy = Mathf.Abs(currentIndex.y - foodIndex.y);
                    if (dx <= 1 && dy <= 1)
                    {
                        // 隣のセルに食料がある場合は移動しないので、現在地を経路として追加する
                        _path.Add(Context.Transform.position);
                        _field.SetActorOnCell();
                        return true;
                    }
                    else
                    {
                        // 対象のセル + 周囲八近傍に対して経路探索
                        foreach (Vector2Int dir in Utility.SelfAndEightDirections)
                        {
                            Vector2Int targetIndex = foodIndex + dir;
                            if (FieldManager.Instance.TryGetPath(currentIndex, targetIndex, out _path))
                            {
                                // 経路の末端(資源のセルの隣)にキャラクターがいる場合は弾く
                                if (FieldManager.Instance.IsActorOnCell(_path[^1], out ActorType _)) continue;
                                
                                _field.SetActorOnCell(_path[^1]);
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// 各値を既定値に戻すことで、現在のセルの位置を自身の位置で更新する。
        /// 次のセルの位置をあれば次のセルの位置、なければ自身の位置で更新する。
        /// </summary>
        /// <returns>次のセルがある:true 次のセルが無い(目的地に到着):false</returns>
        bool TryStepNextCell()
        {
            _move.Reset();

            if (_path.Count > 0)
            {
                // 経路の末尾から1つ取り出す
                _move.NextCellPos = _path[0];
                _path.RemoveAt(0);
                // 経路のセルとキャラクターの高さが違うので水平に移動させるために高さを合わせる
                _move.NextCellPos.y = Context.Transform.position.y;
                
                _move.Modify();
                _move.Look();

                return true;
            }

            _move.NextCellPos = Context.Transform.position;

            return false;
        }

        /// <summary>
        /// 回復の進捗度を進める
        /// </summary>
        /// <returns>回復の進捗中:true 回復の進捗が回復値に達した:false</returns>
        bool StepEatProgress()
        {
            float value = Time.deltaTime * EffectDelta;
            _effectProgress += value;
            //Context.OnEatFoodInvoke(value); // 値の更新

            return _effectProgress <= EffectValue;
        }
    }
}