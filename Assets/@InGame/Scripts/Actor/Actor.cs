using UnityEngine;
using UnityEngine.Events;
using UniRx;

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
    [RequireComponent(typeof(DataContext))]
    public class Actor : MonoBehaviour, IReadOnlyActorStatus
    {
        public static event UnityAction<Actor> OnSpawned;

        [SerializeField] DataContext _context;
        [SerializeField] SkinnedMeshRenderer _renderer;
        [SerializeField] Material _defaultMaterial;

        ActionEvaluator _evaluator;
        SightSensor _sightSensor;
        BaseState _currentState;
        bool _initialized;
        bool _isDead;

        // �L�����N�^�[�̊e��p�����[�^�B�������O�ɓǂݎ�����ꍇ�͉��̒l��Ԃ��B
        public float Food         => _initialized ? _context.Food.Percentage : default;
        public float Water        => _initialized ? _context.Water.Percentage : default;
        public float HP           => _initialized ? _context.HP.Percentage : default;
        public float LifeSpan     => _initialized ? _context.LifeSpan.Percentage : default;
        public float BreedingRate => _initialized ? _context.BreedingRate.Percentage : default;
        public string StateName   => _initialized ? _currentState.Type.ToString() : string.Empty;
        public ActorType Type     => _initialized ? _context.Type : ActorType.None;
        // ���񂾏ꍇ(�v�[���ɕԋp����)�̃t���O
        public bool IsDead => _initialized ? _isDead : false;

        /// <summary>
        /// �X�|�i�[���琶�����ꂽ�ۂɃX�|�i�[�����Ăяo���ď���������K�v������B
        /// </summary>
        public void Init(uint? gene = null) 
        {
            _isDead = false;

            _context.Init(gene);
            ApplyGene();
            _currentState = _context.EvaluateState;
            _evaluator ??= new(_context);
            _sightSensor ??= new(_context);

            OnSpawned?.Invoke(this);
            _initialized = true;
        }

        // ���S����ۂɔ�\���ɂȂ�
        void OnDisable()
        {
            _isDead = true;

            // ���S�������b�Z�[�W�̑��M
            MessageBroker.Default.Publish(new ActorDeathMessage());
        }

        /// <summary>
        /// ��`�q�𔽉f���ăT�C�Y�ƐF��ς���
        /// </summary>
        void ApplyGene()
        {
            _context.Model.localScale *= _context.Size;

            // ���݂̃}�e���A�����폜
            Destroy(_renderer.material);
            // �f�t�H���g�̃R�s�[����}�e���A�����쐬
            Material next = new(_defaultMaterial);
            next.SetColor("_BaseColor", _context.Color);
            _renderer.material = next;
        }

        /// <summary>
        /// �p�����[�^��1�t���[���������ω�������
        /// </summary>
        public void StepParams()
        {
            _context.StepFood();
            _context.StepWater();
            _context.StepLifeSpan();
           
            // �H���Ɛ�����0�ȉ��Ȃ�̗͂����炷
            if (_context.Food.IsBelowZero && _context.Water.IsBelowZero)
            {
                _context.StepHp();
            }
            // �̗͂����ȏ�Ȃ�ɐB������������
            if (_context.IsBreedingRateIncrease)
            {
                _context.StepBreedingRate();
            }
        }

        /// <summary>
        /// ���݂̃X�e�[�g��1�t���[���������X�V����
        /// </summary>
        public void StepAction()
        {
            _currentState = _currentState.Update();
        }

        /// <summary>
        /// ���g�̏��ƃ��[�_�[�̕]���l�����Ɏ��̍s�������߂�B
        /// </summary>
        public void Evaluate(float[] leaderEvaluate)
        {
            // �]���X�e�[�g�ȊO�ł͕]�����Ȃ����ƂŁA���t���[���]���������s���̂�h��
            if (_currentState != _context.EvaluateState) return;

            // ���͂̓G�ƕ������m
            _context.Enemy = _sightSensor.SearchTarget(_context.EnemyTag);
            
            // TODO:�ǉ��̕]�����\�[�X������ꍇ�͂����ɏ���
            
            // ���ɏ�������
            _context.NextAction = _evaluator.SelectAction(leaderEvaluate);
        }

        // �����[�_�[�݂̂̃��\�b�h
        public float[] LeaderEvaluate()
        {
            // �{����public�ȍ������čs����]������
            float[] eval = new float[Utility.GetEnumLength<ActionType>() - 1];
            eval[(int)ActionType.Gather] = 1;

            return eval;
        }
    }

    // �o�O:�o�H��������Ȃ��G���[���o��o�O
    // �o�����: ���̃X�e�[�g���ɐB�X�e�[�g�Ɠ������r���ŉ쎀���E�Q�����悤�C��
    // ���^�X�N: ���[�_�[�����񂾍ۂ̏����A�Q��̒������Ȃ��Ƃ����Ȃ�
    // ���^�X�N: ���[�_�[�����ʂƃ����_���Ŏ��̃��[�_�[�����܂�B�Q��̍Ō��1�C�����ʂƂ��߂��ׂ�
    // ���^�X�N: �̂̋����𐔒l������B�T�C�Y�ƐF�ŋ��߁A�e��]���ɂ͂��̒l���g���B

    // ���낤���Stay���������Stay�H������Enter���Ă΂�Ă��Ȃ��̂�Stay���Ă΂�Ă���B
    // �Ȃ��H������1�x�v�[���ɖ߂��Ă�����o�����̂̂݁B

    // �ɐB���̃o�O(�C���ς݁H)
    // �쎀�X�e�[�g�ɑJ�ڂ�����Ԃł��ɐB���Ƃ��Ďc���Ă��܂��Ă���
    // �Ȃ̂ŉ쎀�X�e�[�g�ł����ȂȂ��H
    // �������A�쎀�X�e�[�g�ɓ���΋����I�Ƀ����_��������������A�\�����Ȃ��Ȃ�͂�
    // �쎀��ԂɂȂ�Ɖ쎀�X�e�[�g�ɑJ�ڂ͂���
    // �}�b�`���O�ҋ@��ԂŃo�O���Ă���
    // �쎀�X�e�[�g�ɑJ�ڂ����܂ܔԂ܂ňړ����Ă���
    // �쎀�X�e�[�g�̂܂ܔɐB�̔Ԃ�҂��Ă���

    // Exit���Ă΂�Ȃ��ŉ쎀�X�e�[�g�ɑJ�ڂ��Ă���H
    // �i��:State��\�����Ă���Ǝv���Ă����e�L�X�g���]�������A�N�V������\�����Ă����B

    // ���Ԋu�ŕ]�� or ���t���[���]��
    //  �X�e�[�g�̍X�V�^�C�~���O�̓Z���̏�ɂ��鎞
    //  ���t���[���]�����Ă����v�H�Ȃ͂�
    //  ���[�_�[����̖��߂͂ǂ�����H�X�e�[�g�̍X�V�^�C�~���O�͈��Ԋu�Ȃ̂ŁA
    //   1�t���[�������̖��߂̑��M�͐���ɓ����Ȃ��\��������B

    // �X�e�[�g�}�V���͂ǂ�����H
    // �]�� -> �s�� ->
    // �^�C�~���O�ɂ��āB
    // �]�� -> �������ނ͓���t���[���ōs���B

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

    // �L�����N�^�[:����
    // ���[�_�[�����Ȃ��B�e�X������ɍs������B
    // ������������ƍU�����Ă���B
    // �U���Ŏ��ʁB
    // �ɐB�͂��Ȃ��B
    // �����_���ŕ����B
    // ���ʂ��тɈ�`�I�ȕψق�����H�����Ȃ�����キ�Ȃ�����
}