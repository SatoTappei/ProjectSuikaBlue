using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class MaleBreedState : BaseState
    {
        enum Stage
        {
            Move,
            Mating,
        }

        readonly MoveModule _move;
        readonly FieldModule _field;
        readonly DetectModule _detect;
        readonly RuleModule _rule;

        Stage _stage;
        bool _firstStep; // 経路のスタート地点から次のセルに移動中
        float _matingTimer;
        bool _isMating;  // 交尾中
        bool _completed; // 交尾完了

        public MaleBreedState(DataContext context) : base(context, StateType.MaleBreed) 
        {
            _move = new(context);
            _field = new(context);
            _detect = new(context);
            _rule = new(context);
        }

        Collider[] Detected => Context.Detected;
        List<Vector3> Path => Context.Path;
        Vector3 Position => Context.Transform.position;
        float MatingTime => Context.Base.MatingTime;
        uint Gene => Context.Gene;
        float BreedingRate { set => Context.BreedingRate.Value = value; }

        protected override void Enter()
        {
            _move.Reset();
            _move.TryStepNextCell();
            _field.SetOnCell(_move.CurrentCellPos);
            _stage = Stage.Move;
            _firstStep = true;
            _matingTimer = 0;
            _isMating = false;
            _completed = false;
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

                    // 死亡した場合は遷移する
                    if (_rule.IsDead()) { ToEvaluateState(); return; }

                    // 経路の末端(雌と同じもしくは隣のセル)まで辿り着いた場合は交尾
                    if (!_move.TryStepNextCell()) _stage = Stage.Mating;
                }
                else
                {
                    _move.Move();
                }
            }
            // 交尾
            else if (_stage == Stage.Mating)
            {
                // 繁殖相手の隣に移動完了、周囲八近傍の雌に対して交尾
                if (!_isMating)
                {
                    _isMating = true;
                    // 交尾する雌がいない場合は評価ステートに戻る
                    if (!CallNeighbourFemale()) { ToEvaluateState(); return; }
                }

                // 交尾中に交尾にかかる時間以上経過した場合は、交尾失敗とみなし評価ステートに戻る
                if (_isMating) _matingTimer += Time.deltaTime;
                if (_matingTimer > MatingTime)
                {
                    // 連続で繁殖ステートに遷移してこないように繁殖率を50％にする
                    BreedingRate = StatusBase.Max / 2;
                    ToEvaluateState();
                    return;
                }

                // 交尾中は死んでも集合でも他のステートに遷移しない？

                // 交尾完了フラグが立った場合は評価ステートに戻る
                if (_completed)
                {
                    BreedingRate = 0;
                    ToEvaluateState();
                    return;
                }
            }
        }

        /// <summary>
        /// 周囲八近傍の繁殖ステートの雌を検知する
        /// </summary>
        /// <returns>番の雌がいる:true いない:false</returns>
        bool DetectPartnerOnNeighbourCell(out Actor actor)
        {
            _detect.OverlapSphere(Utility.NeighbourCellRadius);

            foreach (Collider collider in Detected)
            {
                if (collider == null) break;
                // 雌以外を弾く
                if (!(collider.TryGetComponent(out Actor female) && female.Sex == Sex.Female)) continue;
                // 雌が繁殖ステートの場合のみ
                if (female.State != StateType.FemaleBreed) continue;

                actor = female;
                return true;
            }

            actor = null;
            return false;
        }

        bool CallNeighbourFemale()
        {
            if (DetectPartnerOnNeighbourCell(out Actor actor))
            {
                actor.SpawnChild(Gene, () => _completed = true);
                return true;
            }

            return false;
        }
    }
}