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
        const float TimeOut = 10.0f;
        const float SearchRate = 1.0f;

        FieldModule _field;
        Collider[] _detected = new Collider[8];
        float _timer;
        float _nextSearchTime;

        public FemaleBreedState(DataContext context) : base(context, StateType.FemaleBreed)
        {
            _field = new(context);
        }

        protected override void Enter()
        {
            _field.SetActorOnCell();
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

            // 時間切れ、もしくは食料/水分共に0で評価ステートに遷移
            if (_timer > TimeOut || (Context.Water.Value <= 0 && Context.Food.Value <= 0))
            {
                // 繁殖率を50％にして連続でこのステートに遷移しないようにする
                Context.BreedingRate.Value = StatusBase.Max / 2;
                ToEvaluateState();
                return;
            }
        }

        bool SearchEnemy()
        {
            Array.Clear(_detected, 0, _detected.Length);

            Vector3 pos = Context.Transform.position;
            float radius = Context.Base.SightRadius;
            LayerMask layer = Context.Base.SightTargetLayer;
            if (Physics.OverlapSphereNonAlloc(pos, radius, _detected, layer) == 0) return false;

            // 近い順に配列に入っているので、一番近い敵を対象の敵として書き込む。
            foreach (Collider collider in _detected)
            {
                if (collider == null) return false;
                if (collider.CompareTag(Context.EnemyTag)) return true;
            }

            return false;
        }

        void PlayParticle()
        {
            MessageBroker.Default.Publish(new PlayParticleMessage()
            {
                Type = ParticleType.MatingReady,
                Pos = Context.Transform.position,
            });
        }
    }
}
