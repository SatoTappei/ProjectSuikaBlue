using System;
using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// このステート自体が繁殖の処理を持つのではなく
    /// 交尾->出産の処理はActorクラスに書かれており、このステートは繁殖の処理を受け付けるだけ。
    /// </summary>
    public class FemaleBreedState : BaseState
    {
        readonly FieldModule _field;
        readonly DetectModule _detect;
        readonly RuleModule _rule;

        float _timer;
        float _nextSearchTime;

        public FemaleBreedState(DataContext context) : base(context, StateType.FemaleBreed)
        {
            _field = new(context);
            _detect = new(context);
            _rule = new(context);
        }

        Collider[] Deteceted => Context.Detected;
        Vector3 Position => Context.Transform.position;
        string EnemyTag => Context.EnemyTag;
        float Radius => Context.Base.SightRadius;
        float TimeOut => Context.Base.TimeOut;
        float SearchRate => Context.Base.SearchRate;
        float BreedingRate { set => Context.BreedingRate.Value = value; }

        protected override void Enter()
        {
            _field.SetOnCell();
            _timer = 0;
            _nextSearchTime = SearchRate;
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            _timer += Time.deltaTime;

            // 一定間隔で敵を検知し、敵が周囲にいた場合は評価ステートに遷移
            if (_timer > _nextSearchTime)
            {
                if (SearchEnemy()) { ToEvaluateState(); return; }
                else _nextSearchTime += SearchRate;

                PlayParticle();
            }

            // 時間切れ、もしくは食料/水分共に0、もしくは死亡で評価ステートに遷移
            if (_timer > TimeOut || _rule.IsHunger() || _rule.IsDead())
            {
                // 繁殖率を50％にして連続でこのステートに遷移しないようにする
                BreedingRate = StatusBase.Max / 2;
                ToEvaluateState();
                return;
            }
        }

        bool SearchEnemy()
        {
            _detect.OverlapSphere(Radius);

            // 近い順に配列に入っているので、一番近い敵を対象の敵として書き込む。
            foreach (Collider collider in Deteceted)
            {
                if (collider == null) return false;
                if (collider.CompareTag(EnemyTag)) return true;
            }

            return false;
        }

        void PlayParticle()
        {
            MessageBroker.Default.Publish(new PlayParticleMessage()
            {
                Type = ParticleType.MatingReady,
                Pos = Position,
            });
        }
    }
}
