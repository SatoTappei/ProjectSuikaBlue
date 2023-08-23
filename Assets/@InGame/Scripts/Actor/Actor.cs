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
    [RequireComponent(typeof(BlackBoard))]
    public class Actor : MonoBehaviour, IReadOnlyParams, IReadOnlyBreedingParam
    {
        public static event UnityAction<Actor> OnSpawned;

        [SerializeField] InitializeProcess _initProcess;
        [SerializeField] ActionEvaluator _evaluator;
        // StatusBase�̎擾��Controller���ł̐���ɕK�v�Ȃ̂Ō̖��Ƀf�[�^������
        [SerializeField] ActorType _type;

        IBlackBoardForActor _blackBoard;
        Status _status;
        BaseState _currentState;
        string _name;
        bool _initialized;

        public ActorType Type => _type;
        // �ǂݎ��p�B�������O�ɓǂݎ�����ꍇ�͉��̒l�Ƃ���1��Ԃ��B
        float IReadOnlyParams.Food         => _initialized ? _status.Food.Percentage : 1;
        float IReadOnlyParams.Water        => _initialized ? _status.Water.Percentage : 1;
        float IReadOnlyParams.HP           => _initialized ? _status.Hp.Percentage : 1;
        float IReadOnlyParams.LifeSpan     => _initialized ? _status.LifeSpan.Percentage : 1;
        float IReadOnlyParams.BreedingRate => _initialized ? _status.BreedingRate.Percentage : 1;
        string IReadOnlyEvaluate.ActionName => _initialized ? _blackBoard.NextAction.ToString() : string.Empty;
        string IReadOnlyObjectInfo.Name => _initialized ? _name ??= gameObject.name : string.Empty;
        // �ɐB�X�e�[�g���ǂݎ��p�B
        uint IReadOnlyBreedingParam.Gene => _status.Gene;

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
            // ���[�_�[�̊e�s���ւ̕]���Ƃ̍��Z�őI������
            float[] myEvaluate = _evaluator.Evaluate(_status);
            ActionType action = ActionEvaluator.SelectMax(myEvaluate, leaderEvaluate);

            // ���ɏ�������
            _blackBoard.NextAction = action;
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
    }

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