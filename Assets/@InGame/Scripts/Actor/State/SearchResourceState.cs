using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

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
        readonly RuleModule _rule;
        readonly EatParticleModule _particle;
        readonly ResourceType _resourceType;
        readonly UnityAction _stepEatAction;

        float _healingProgress;
        Stage _stage;
        bool _firstStep;

        public SearchResourceState(DataContext context, StateType stateType, ResourceType resourceType, 
            UnityAction stepEatAction) : base(context, stateType)
        {
            _move = new(context);
            _field = new(context);
            _particle = new(context);
            _rule = new(context);
            _resourceType = resourceType;
            _stepEatAction = stepEatAction;
        }

        public List<Vector3> Path => Context.Path;
        Vector3 Position => Context.Transform.position;
        public float HealingRate => Context.Base.HealingRate;

        protected override void Enter()
        {
            _move.Reset();
            _move.TryStepNextCell();
            _field.SetOnCell(_move.CurrentCellPos);
            _particle.Reset();
            _stage = Stage.Move;
            _healingProgress = 0;
            _firstStep = true;
        }

        protected override void Exit()
        {
            _field.DeletePathGoalOnCell();
            Path.Clear();
        }

        protected override void Stay()
        {
            // 移動
            if (_stage == Stage.Move)
            {
                if (_move.OnNextCell)
                {
                    if (_firstStep)
                    {
                        _firstStep = false;
                        _field.DeleteOnCell(_move.CurrentCellPos);
                    }

                    if (_rule.IsDead()) { ToEvaluateState(); return; }

                    if (!_move.TryStepNextCell())
                    {
                        _stage = Stage.Eat;
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
        /// 回復の進捗度を進める
        /// </summary>
        /// <returns>回復の進捗中:true 回復の進捗が回復値に達した:false</returns>
        bool StepEatProgress()
        {
            _stepEatAction?.Invoke();
            _healingProgress += Time.deltaTime * HealingRate;
            int limit = FieldManager.Instance.Resource[_resourceType].HealingLimit;
            return _healingProgress <= limit;
        }
    }
}