using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

# region ����
// �o�����: ���̃X�e�[�g���ɐB�X�e�[�g�Ɠ������r���ŉ쎀���E�Q�����悤�C��
// �o�����: �������ɂ�鐶���̔��肪�o�O���Ă���
// �ύX��: �̂̋����𐔒l������B�T�C�Y�ƐF�ŋ��߁A�e��]���ɂ͂��̒l���g���B
// �ύX��: 1�ł͂Ȃ��A�D��x�Ń\�[�g���Ď��̍s����ێ��B
// �o�O: �W����I�������ۂɁA�o�H�̖��[��2�̂̃L�����N�^�[������Ă��܂��B�������9�l�Ŕ���

// ���U��
// 1�Α��̏󋵂͂ǂ�����H
// �G��|�����ۂɂ��]���X�e�[�g�ɑJ�ڂ���K�v������B
//  ��1:1����������]���X�e�[�g�ɑJ�� <- �̗͂��������玩���œ�����͂��B
//  �E�����ꍇ�͓G�����m�������̃X�e�[�g�ɑJ�ڂ��Ă��Ȃ��͂��B
//  �K�v�Ȓl:�����X�R�A(�F�A�T�C�Y) <- ���̒l�ɂ���čU���͂��ς��
// �����[�_�[
// ���[�_�[�����ʂƃ����_���Ŏ��̃��[�_�[�����܂�B
// �Q��̍Ō��1�C�����ʂƂ��߂��ׂ�
// ���H��/����
// ���Ԋu�Ő����ƐH���̂������Ȃ��ق��𖞂������Ƃ���
// ��̓I�ɂ͉��񂩃Z�����ړ�������`�F�b�N
// ��������Ă��邩�`�F�b�N�B��������Ă���ꍇ�͉������Ȃ��B
// �߂��̐����������͐H���̃}�X���擾
// �n���Ōo�H�T���B�o�H������Ό������B�����ꍇ�͂Ȃɂ����Ȃ�
// ���s��
// �s���̒P�ʂ̓A�j���[�V����
// �A�j���[�V�������I������玟�̍s����I������
// �܂�A�s��A -> ���f -> �s��B �ƍs�����ɒ����̏�Ԃɖ߂��Ď��̍s�����`�F�b�N����K�v������B
// �ʏ�̃X�e�[�g�x�[�X�ł͖����B
// �O������̍U���Ȃǂōs�����Ɏ��ʏꍇ������H
// ���ɐB
// �ɐB����ۂ͐e�̓������󂯌p��(��`)
// ��`�I�A���S���Y��
// ��`�q��4�ARGB+�T�C�Y�A
// ���G
// �L�����N�^�[:����
// ���[�_�[�����Ȃ��B�e�X������ɍs������B
// ������������ƍU�����Ă���B
// �U���Ŏ��ʁB
// �ɐB�͂��Ȃ��B
// �����_���ŕ����B
// ���ʂ��тɈ�`�I�ȕψق�����H�����Ȃ�����キ�Ȃ�����
#endregion

namespace PSB.InGame
{
    public enum ActorType
    {
        None,
        Kinpatsu,
        KinpatsuLeader,
        Kurokami,
    }

    public enum Sex
    {
        Male,
        Female,
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
        [SerializeField] ChildSpawnBehavior _childSpawn;
        [SerializeField] SkinnedMeshRenderer _renderer;
        [SerializeField] Material _defaultMaterial;
        [SerializeField] GameObject _leaderMarker;

        ActionEvaluator _evaluator;
        LeaderEvaluator _leaderEvaluator;
        Pathfinder _pathfinder;
        BaseState _currentState;
        bool _initialized; // Init���\�b�h���Ăяo���ꂽ�t���O
        bool _isDead;      // ���S(��\���ɂȂ���)�t���O

        // �������O�ɓǂݎ�����ꍇ�͉��̒l��Ԃ��B
        public float Food         => _initialized ? _context.Food.Percentage : default;
        public float Water        => _initialized ? _context.Water.Percentage : default;
        public float HP           => _initialized ? _context.HP.Percentage : default;
        public float LifeSpan     => _initialized ? _context.LifeSpan.Percentage : default;
        public float BreedingRate => _initialized ? _context.BreedingRate.Percentage : default;
        public StateType State    => _initialized ? _currentState.Type : StateType.Base;
        public ActorType Type     => _initialized ? _context.Type : ActorType.None;
        public Sex Sex            => _initialized ? _context.Sex : Sex.Male;
        public int Score          => _initialized ? _context.Score : default;
        public bool IsDead        => _initialized ? _isDead : false;

        public bool IsLeader
        {
            get => _context.IsLeader;
            set
            {
                _context.IsLeader = value;
                _leaderMarker.SetActive(_context.IsLeader);
            }
        }

        /// <summary>
        /// �X�|�i�[���琶�����ꂽ�ۂɃX�|�i�[�����Ăяo���ď���������K�v������B
        /// </summary>
        public void Init(uint? gene = null) 
        {
            _isDead = false;
            IsLeader = false;

            _context.Init(gene);
            ApplyGene();
            _currentState = _context.EvaluateState;
            _evaluator ??= new(_context);
            _leaderEvaluator ??= new(_context);
            _pathfinder ??= new(_context);
            // �����ʒu�̃Z����ɋ���Ƃ���������������
            FieldManager.Instance.SetActorOnCell(transform.position, _context.Type);
            // �킩��₷���悤�ɖ��O�ɐ��ʂ𔽉f���Ă���
            name += _context.Sex == Sex.Male ? "��" : "��";

            OnSpawned?.Invoke(this);
            MessageBroker.Default.Publish(new ActorSpawnMessage() { Type = _context.Type });
            
            // �����������t���O
            _initialized = true;
        }

        void OnDisable()
        {
            // ���S����ۂɔ�\���ɂȂ�
            _isDead = true;
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
        public void StepAction() => _currentState = _currentState.Update();

        /// <summary>
        /// �]���X�e�[�g�������ꍇ�Ƀ��Z�b�g���鏈��
        /// </summary>
        public void ResetOnEvaluateState()
        {
            // ������������ꍇ�͒��~
            _childSpawn.Cancel();
        }

        /// <summary>
        /// �]���X�e�[�g�ȊO�ł͕]�����Ȃ����ƂŁA���t���[���]���������s���̂�h��
        /// ���g�̏��ƃ��[�_�[�̕]���l�����Ɏ��̍s�������߂�B
        /// </summary>
        public void Evaluate(float[] leaderEvaluate)
        {

            // �G�ɑ_���Ă���ꍇ�́A�U���������͓����邱�Ƃ��ŗD��Ȃ̂ŁA�]���l����ɔ��肷��
            _pathfinder.SearchEnemy();

            // �]���l��p���Ď��̍s����I��
            ActionType action = _evaluator.SelectAction(leaderEvaluate);
            switch (action)
            {
                // �E�Q
                case ActionType.Killed:
                    _context.NextAction = ActionType.Killed; return;
                // ����
                case ActionType.Senility:
                    _context.NextAction = ActionType.Senility; return;
                // �U��
                case ActionType.Attack when _pathfinder.TryPathfindingToEnemy():
                    _context.NextAction = ActionType.Attack; return;
                // ������
                case ActionType.Escape when _pathfinder.TryPathfindingToEscapePoint():
                    _context.NextAction = ActionType.Escape; return;
                // �Y�ɐB
                case ActionType.Breed when _context.Sex == Sex.Male && _pathfinder.TryDetectPartner():
                    _context.NextAction = ActionType.Breed; return;
                // ���ɐB
                case ActionType.Breed when _context.Sex == Sex.Female:
                    _context.NextAction = ActionType.Breed; return;
                // ����
                case ActionType.SearchWater when _pathfinder.TryPathfindingToResource(ResourceType.Water):
                    _context.NextAction = ActionType.SearchWater; return;
                // �H��
                case ActionType.SearchFood when _pathfinder.TryPathfindingToResource(ResourceType.Tree):
                    _context.NextAction = ActionType.SearchFood; return;
                // �W��
                case ActionType.Gather when _pathfinder.TryPathfindingToGatherPoint():
                    _context.NextAction = ActionType.Gather; return;
            }

            // ���낤��X�e�[�g�ɕK�v�̂Ȃ��Q�Ƃ�����
            _context.Enemy = null;
            //_context.Path.Clear();

            // �����_���ɗׂ̃Z���Ɉړ�����
            _context.NextAction = ActionType.Wander;
        }

        /// <summary>
        /// �ɐB���s��
        /// �Y�����̂��̃��\�b�h���Ăяo��
        /// </summary>
        public void SpawnChild(uint maleGene, UnityAction callback = null)
        {
            if (_currentState.Type != StateType.FemaleBreed)
            {
                Debug.LogWarning($"{name} ���̔ɐB�X�e�[�g�ȊO�Ŏq�����Y�ޏ������Ă΂�Ă���B");
            }
            else
            {
                _childSpawn.SpawnChild(maleGene, callback);
            }
        }

        /// <summary>
        /// ���[�_�[�Ƃ��Ă̕]�����s��
        /// �Q��̋��ʂ̍���p���ĕ]�����s��
        /// </summary>
        /// <returns>�e�s���ɑ΂��Ă̕]���l</returns>
        public float[] LeaderEvaluate() => _leaderEvaluator.Evaluate();

        void OnDrawGizmos()
        {
            // ���͔��ߖT�̃Z���̋�����`��
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, Utility.NeighbourCellRadius);
        }
    }
}