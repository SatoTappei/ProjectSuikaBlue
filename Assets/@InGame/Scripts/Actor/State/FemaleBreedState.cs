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

        FieldModule _field;
        float _timer;

        public FemaleBreedState(DataContext context) : base(context, StateType.FemaleBreed)
        {
            _field = new(context);
        }

        protected override void Enter()
        {
            _field.SetActorOnCell();
            _timer = 0;
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            // ���Ԑ؂�ŕ]���X�e�[�g�ɑJ��
            _timer += Time.deltaTime;
            if (_timer > TimeOut)
            {
                // �ɐB����50���ɂ��ĘA���ł��̃X�e�[�g�ɑJ�ڂ��Ȃ��悤�ɂ���
                Context.BreedingRate.Value = StatusBase.Max / 2;
                ToEvaluateState();
                return;
            }
        }
    }
}
