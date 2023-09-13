using System;
using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// ���̃X�e�[�g���̂��ɐB�̏��������̂ł͂Ȃ�
    /// ���->�o�Y�̏�����Actor�N���X�ɏ�����Ă���A���̃X�e�[�g�͔ɐB�̏������󂯕t���邾���B
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

            // ���Ԋu�œG�����m���A�G�����͂ɂ����ꍇ�͕]���X�e�[�g�ɑJ��
            if (_timer > _nextSearchTime)
            {
                if (SearchEnemy()) { ToEvaluateState(); return; }
                else _nextSearchTime += SearchRate;

                PlayParticle();
            }

            // ���Ԑ؂�A�������͐H��/��������0�ŕ]���X�e�[�g�ɑJ��
            if (_timer > TimeOut || (Context.Water.Value <= 0 && Context.Food.Value <= 0))
            {
                // �ɐB����50���ɂ��ĘA���ł��̃X�e�[�g�ɑJ�ڂ��Ȃ��悤�ɂ���
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

            // �߂����ɔz��ɓ����Ă���̂ŁA��ԋ߂��G��Ώۂ̓G�Ƃ��ď������ށB
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
