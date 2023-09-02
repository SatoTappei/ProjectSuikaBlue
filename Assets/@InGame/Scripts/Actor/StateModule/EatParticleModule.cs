using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// ���Ԋu�ŐH���̃p�[�e�B�N�������̃��b�Z�[�W�𑗐M����
    /// �L�����N�^�[�̎q�Ƃ��Đ�������A�v���[�`�ƈႢ
    /// �L�����N�^�[����A�N�e�B�u�ɂȂ����u�Ԃɏ�����Ȃǂ̋������h����
    /// </summary>
    public class EatParticleModule
    {
        // �H�����̉��o�̃p�[�e�B�N�����o���Ԋu
        const float PlayParticleRate = 0.5f;

        DataContext _context;
        float _timer;

        public EatParticleModule(DataContext context)
        {
            _context = context;
        }

        public void Reset()
        {
            _timer = PlayParticleRate;
        }

        public void Update()
        {
            _timer += Time.deltaTime;
            if (_timer > PlayParticleRate)
            {
                _timer = 0;
                PlayParticle();
            }
        }

        void PlayParticle()
        {
            MessageBroker.Default.Publish(new PlayParticleMessage()
            {
                Type = ParticleType.Eat,
                Pos = _context.Transform.position,
            });
        }
    }
}