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

            // ���Ԋu�œG�����m���A�G�����͂ɂ����ꍇ�͕]���X�e�[�g�ɑJ��
            if (_timer > _nextSearchTime)
            {
                if (SearchEnemy()) { ToEvaluateState(); return; }
                else _nextSearchTime += SearchRate;

                PlayParticle();
            }

            // ���Ԑ؂�A�������͐H��/��������0�A�������͎��S�ŕ]���X�e�[�g�ɑJ��
            if (_timer > TimeOut || _rule.IsHunger() || _rule.IsDead())
            {
                // �ɐB����50���ɂ��ĘA���ł��̃X�e�[�g�ɑJ�ڂ��Ȃ��悤�ɂ���
                BreedingRate = StatusBase.Max / 2;
                ToEvaluateState();
                return;
            }
        }

        bool SearchEnemy()
        {
            _detect.OverlapSphere(Radius);

            // �߂����ɔz��ɓ����Ă���̂ŁA��ԋ߂��G��Ώۂ̓G�Ƃ��ď������ށB
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
