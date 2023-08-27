using UnityEngine;
using UnityEngine.Events;
using UniRx;
using System;
using System.Buffers;

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
    [RequireComponent(typeof(InitializeProcess))]
    [RequireComponent(typeof(ActionEvaluator))]
    [RequireComponent(typeof(SightSensor))]
    [RequireComponent(typeof(BlackBoard))]
    public class Actor : MonoBehaviour, IReadOnlyParams, IReadOnlyBreedingParam, IReadOnlyGeneParams
    {
        public static event UnityAction<Actor> OnSpawned;

        [SerializeField] InitializeProcess _initProcess;
        [SerializeField] ActionEvaluator _evaluator;
        [SerializeField] SightSensor _sightSensor;
        // StatusBase�̎擾��Controller���ł̐���ɕK�v�Ȃ̂Ō̖��Ƀf�[�^������
        [SerializeField] ActorType _type;

        IBlackBoardForActor _blackBoard;
        Status _status;
        BaseState _currentState;
        string _name;
        bool _initialized;

        public Transform Leader { set => _blackBoard.Leader = value; } // �e�X�g�p�A�X�e�[�g�Ń��[�_�[���Q�Ƃ��邽�߂ɕK�v
        public ActorType Type => _type;
        // �ǂݎ��p�B�������O�ɓǂݎ�����ꍇ�͉��̒l�Ƃ���1��Ԃ��B
        float IReadOnlyParams.Food         => _initialized ? _status.Food.Percentage : 1;
        float IReadOnlyParams.Water        => _initialized ? _status.Water.Percentage : 1;
        float IReadOnlyParams.HP           => _initialized ? _status.Hp.Percentage : 1;
        float IReadOnlyParams.LifeSpan     => _initialized ? _status.LifeSpan.Percentage : 1;
        float IReadOnlyParams.BreedingRate => _initialized ? _status.BreedingRate.Percentage : 1;
        string IReadOnlyEvaluate.StateName => _initialized ? _currentState.Type.ToString() : string.Empty;
        string IReadOnlyObjectInfo.Name => _initialized ? _name ??= gameObject.name : string.Empty;
        // �ɐB�X�e�[�g���ǂݎ��p�B
        uint IReadOnlyBreedingParam.Gene => _status.Gene;
        // �]���N���X���ǂݎ��p�B
        byte IReadOnlyGeneParams.ColorR => _status.ColorR;
        byte IReadOnlyGeneParams.ColorG => _status.ColorG;
        byte IReadOnlyGeneParams.ColorB => _status.ColorB;
        Color32 IReadOnlyGeneParams.Color => _status.Color;
        float IReadOnlyGeneParams.Size => _status.Size;

        /// <summary>
        /// �X�|�i�[�������̃^�C�~���O�ŌĂԏ���������
        /// </summary>
        public void Init(uint? gene = null) 
        {
            // ������������A�e��X�e�[�^�X�̒l���Q�Ƃł���B
            _status = _initProcess.Execute(gene, _type);
            _initialized = true;

            // FSM�̏����B�]���X�e�[�g��������Ԃ̃X�e�[�g�Ƃ���B
            _blackBoard = GetComponent<BlackBoard>();
            _currentState = _blackBoard.InitState;

            // �H�ׂ�/���ރX�e�[�g���X�e�[�^�X��ω�������悤�ɓo�^����
            _blackBoard.OnEatFoodRegister(v => _status.Food.Value += v);
            _blackBoard.OnDrinkWaterRegister(v => _status.Water.Value += v);

            // �ɐB�X�e�[�g�����g���Y/���̎��ɍs�����������ꂼ��o�^����
            _blackBoard.OnFemaleBreedingRegister(SendSpawnChildMessage);
            _blackBoard.OnFemaleBreedingRegister(_ => _status.BreedingRate.Value = 0);
            _blackBoard.OnMaleBreedingRegister(() => _status.BreedingRate.Value = 0);

            OnSpawned?.Invoke(this);
        }

        /// <summary>
        /// ���t���[���Ăяo����A�Ăяo����邽�тɃp�����[�^��1�t���[���������ω�������
        /// </summary>
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

        /// <summary>
        /// ���t���[���Ăяo����A�Ăяo����邽�тɌ��݂̃X�e�[�g��1�t���[���������X�V����
        /// </summary>
        public void StepAction()
        {
            _currentState = _currentState.Update();
        }

        public void Evaluate()
        {
            // �_�~�[������ČĂяo��
            int length = Utility.GetEnumLength<ActionType>() - 1;
            float[] buffer = ArrayPool<float>.Shared.Rent(length);
            float[] dummy = buffer.AsSpan(0, length).ToArray();

            Evaluate(dummy);

            ArrayPool<float>.Shared.Return(buffer);
        }

        public void Evaluate(float[] leaderEvaluate)
        {
            // ���͂̓G�ƕ������m
            Actor enemy = _sightSensor.SearchEnemy();

            // ���[�_�[�̊e�s���ւ̕]���Ƃ̍��Z�őI������
            float[] myEvaluate = _evaluator.Evaluate(_status, enemy);
            ActionType action = ActionEvaluator.SelectMax(myEvaluate, leaderEvaluate);

            // ���ɏ�������
            _blackBoard.NextAction = action;
            _blackBoard.Enemy = enemy;
        }

        void SendSpawnChildMessage(uint gene)
        {
            MessageBroker.Default.Publish(new SpawnChildMessage 
            {
                Gene1 = gene,
                Gene2 = _status.Gene,
                Params = this,
                Pos = transform.position, // ���g�̈ʒu�ɐ�������
            });
        }

        public void Damaged()
        {
            _status.Hp.Value -= 10;
        }

        public float[] LeaderEvaluate()
        {
            // �{����public�ȍ������čs����]������
            float[] eval = new float[Utility.GetEnumLength<ActionType>() - 1];
            eval[(int)ActionType.Gather] = 1;

            return eval;
        }
    }

    // �o�O:�o�H��������Ȃ��G���[���o��o�O
    // �o�����:���̃X�e�[�g���ɐB�X�e�[�g�Ɠ������r���ŉ쎀���E�Q�����悤�C��
    // ���^�X�N:���[�_�[�����񂾍ۂ̏����A�Q��̒������Ȃ��Ƃ����Ȃ�
    // ���^�X�N:���[�_�[���߂ŌQ����W�������鏈��
    // ���^�X�N:�̂̋����𐔒l������B�T�C�Y�ƐF�ŋ��߁A�e��]���ɂ͂��̒l���g��
    
    // �U���Ɋւ��Ă̓L�����N�^�[�ȊO�ɓG�����͂ɂ���K�v������B
    // �G�����m-> �]�� 

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

    // ����
    // ���[�_�[�����Ȃ��B�e�X������ɍs������B
    // ������������ƍU�����Ă���B
    // �U���Ŏ��ʁB
    // �ɐB�͂��Ȃ��B
    // �����_���ŕ����B
    // ���ʂ��тɈ�`�I�ȕψق�����H�����Ȃ�����キ�Ȃ�����
}