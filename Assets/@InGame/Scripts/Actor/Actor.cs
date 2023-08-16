using UnityEngine;
using UnityEngine.Events;

namespace PSB.InGame
{
    public enum ActorType
    {
        None,
        Kinpatsu,
        KinpatsuLeader,
        Kurokami,
    }

    /// <summary>
    /// �X�|�i�[���琶������AController�ɂ���đ��삳���B
    /// �P�̂œ��삷�邱�Ƃ��l�����Ă��Ȃ��̂ŃV�[����ɒ��ɔz�u���Ă��@�\���Ȃ��B
    /// </summary>
    [RequireComponent(typeof(ChildSpawner))]
    [RequireComponent(typeof(InitializeProcess))]
    public class Actor : MonoBehaviour, IReadOnlyParams
    {
        public static event UnityAction<Actor> OnSpawned;

        [SerializeField] ChildSpawner _spawner;
        [SerializeField] InitializeProcess _initProcess;
        // StatusBase�̎擾��Controller���ł̐���ɕK�v�Ȃ̂Ō̖��Ƀf�[�^������
        [SerializeField] ActorType _type;

        Status _status;
        bool _initialized;

        public ActorType Type => _type;
        // UI�����ǂݎ��p�B�������O�ɓǂݎ�����ꍇ�͉��̒l�Ƃ���1��Ԃ��B
        float IReadOnlyParams.Food         => _initialized ? _status.Food.Percentage : 1;
        float IReadOnlyParams.Water        => _initialized ? _status.Water.Percentage : 1;
        float IReadOnlyParams.HP           => _initialized ? _status.Hp.Percentage : 1;
        float IReadOnlyParams.LifeSpan     => _initialized ? _status.LifeSpan.Percentage : 1;
        float IReadOnlyParams.BreedingRate => _initialized ? _status.BreedingRate.Percentage : 1;

        /// <summary>
        /// �X�|�i�[�������̃^�C�~���O�ŌĂԏ���������
        /// </summary>
        public void Init(uint? gene = null) 
        {
            // ������������A�e��X�e�[�^�X�̒l���Q�Ƃł���B
            _status = _initProcess.Execute(gene, _type);
            _initialized = true;

            OnSpawned?.Invoke(this);
        }

        public void StepParams()
        {
            _status.StepFood();
            _status.StepWater();
            _status.StepLifeSpan();
           
            // �H���Ɛ�����0�ȉ��Ȃ�̗͂����炷
            if (_status.Food.IsBelowZero && _status.Water.IsBelowZero)
            {
                _status.StepHp();
            }
            // �̗͂����ȏ�Ȃ�ɐB������������
            if (_status.IsBreedingRateIncrease)
            {
                _status.StepBreedingRate();
            }
        }

        public void Move()
        {
            transform.Translate(Vector3.forward * Time.deltaTime);
        }
    }

    // ����: ���Ԍo�߂Ō���A����ŉ�
    // �H��: ���Ԍo�߂Ō���A�H�ׂĉ�
    // �̗�: 0�ɂȂ�Ǝ���
        // �����ƐH����0�̎��͌����Ă���
        // �U�������Ƒ̗͂�����
        // �����Ƒ̗͂���������Ă���ꍇ�A�񕜂��Ă���
    // �ɐB��: �̗͂�������Ԃ��Ɖ��Z����Ă����B�ɐB�����烊�Z�b�g�����
        // ��x�ɐB�����͔̂ɐB���Ȃ��悤�ɂ���Ǝq�������񂾂�Q�[�����l��
    // ����: 0�ɂȂ����玀�ʁB���̃p�����[�^�ɉe�����ꂸ��Ɉ��ʌ������Ă����B

    // ���[�_�[�����ʂƃ����_���Ŏ��̃��[�_�[�����܂�B
        // �Q��̍Ō��1�C�����ʂƂ��߂��ׂ�

    // ���Ԋu�Ő����ƐH���̂������Ȃ��ق��𖞂������Ƃ���
        // ��̓I�ɂ͉��񂩃Z�����ړ�������`�F�b�N
        // ��������Ă��邩�`�F�b�N�B��������Ă���ꍇ�͉������Ȃ��B
        // �߂��̐����������͐H���̃}�X���擾
        // �n���Ōo�H�T���B�o�H������Ό������B�����ꍇ�͂Ȃɂ����Ȃ�

    // �s���̒P�ʂ̓A�j���[�V����
        // �A�j���[�V�������I������玟�̍s����I������
        // �܂�A�s��A -> ���f -> �s��B �ƍs�����ɒ����̏�Ԃɖ߂��Ď��̍s�����`�F�b�N����K�v������B
            // �ʏ�̃X�e�[�g�x�[�X�ł͖����B
            // �O������̍U���Ȃǂōs�����Ɏ��ʏꍇ������H

    // �ɐB����ۂ͐e�̓������󂯌p��(��`)
        // ��`�I�A���S���Y��
        // ��`�q��4�ARGB+�T�C�Y�A

    // ����
        // ���[�_�[�����Ȃ��B�e�X������ɍs������B
        // ������������ƍU�����Ă���B
        // �U���Ŏ��ʁB
        // �ɐB�͂��Ȃ��B
        // �����_���ŕ����B
        // ���ʂ��тɈ�`�I�ȕψق�����H�����Ȃ�����キ�Ȃ�����
}