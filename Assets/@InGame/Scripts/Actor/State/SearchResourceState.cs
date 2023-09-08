using UnityEngine;
using UnityEngine.Events;

namespace PSB.InGame
{
    /// <summary>
    /// 資源のセルまで移動し、設定された効果値だけステータスの対応した値を徐々に回復する。
    /// ステータスのパラメータが効果値を上回っていても、効果値分の回復処理が実行される。
    /// </summary>
    public class SearchResourceState : BaseState
    {
        enum Stage
        {
            Move,
            Eat,
        }

        readonly MoveModule _move;
        readonly FieldModule _field;
        readonly EatParticleModule _particle;
        readonly ResourceType _resourceType;
        readonly UnityAction _stepEatAction;
        Stage _stage;
        float _healingProgress;
        // 経路のスタート地点から次のセルに移動する状態のフラグ
        bool _firstStep;

        public SearchResourceState(DataContext context, StateType stateType, ResourceType resourceType, 
            UnityAction stepEatAction) : base(context, stateType)
        {
            _move = new(context);
            _field = new(context);
            _particle = new(context);
            _resourceType = resourceType;
            _stepEatAction = stepEatAction;
        }

        protected override void Enter()
        {
            _move.Reset();
            TryStepNextCell();
            _field.SetActorOnCell();
            _particle.Reset();
            _stage = Stage.Move;
            _healingProgress = 0;
            _firstStep = true;
        }

        protected override void Exit()
        {
            Context.Path.Clear();
        }

        protected override void Stay()
        {
            // 移動
            if (_stage == Stage.Move)
            {
                if (_move.OnNextCell)
                {
                    // 経路のスタート地点は予約されているので次のセルに移動した際に消す
                    // 全てのセルに対して行うと別のキャラクターで予約したセルまで消してしまう。
                    if (_firstStep)
                    {
                        _firstStep = false;
                        _field.DeleteActorOnCell(_move.CurrentCellPos);
                    }

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
            else if (_stage == Stage.Eat)
            {
                if (!StepEatProgress()) { ToEvaluateState(); return; }

                // 一定間隔でパーティクル
                _particle.Update();
            }
        }

        /// <summary>
        /// 各値を既定値に戻すことで、現在のセルの位置を自身の位置で更新する。
        /// 次のセルの位置をあれば次のセルの位置、なければ自身の位置で更新する。
        /// </summary>
        /// <returns>次のセルがある:true 次のセルが無い(目的地に到着):false</returns>
        bool TryStepNextCell()
        {
            _move.Reset();

            if (Context.Path.Count > 0)
            {
                // 経路の末尾から1つ取り出す
                _move.NextCellPos = Context.Path[0];
                Context.Path.RemoveAt(0);
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
            _stepEatAction?.Invoke();
            _healingProgress += Time.deltaTime * Context.Base.HealingRate;
            return _healingProgress <= FieldManager.Instance.GetResourceHealingLimit(_resourceType);
        }
    }
}