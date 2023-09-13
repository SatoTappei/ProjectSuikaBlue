using UnityEngine;
using UnityEngine.Events;
using UniRx;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

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
        [SerializeField] SkinnedMeshRenderer _renderer;
        [SerializeField] Material _defaultMaterial;
        [SerializeField] GameObject _leaderMarker;

        ActionEvaluator _evaluator;
        LeaderEvaluator _leaderEvaluator;
        BaseState _currentState;
        Collider[] _detected = new Collider[8];
        Coroutine _spawnChild; // ������L�����Z���\�ɂ��邽��
        SpawnChildMessage _spawnChildMessage = new();
        bool _initialized;
        bool _isDead;
        bool _isMating; // ������t���O

        // �L�����N�^�[�̊e��p�����[�^�B�������O�ɓǂݎ�����ꍇ�͉��̒l��Ԃ��B
        public float Food         => _initialized ? _context.Food.Percentage : default;
        public float Water        => _initialized ? _context.Water.Percentage : default;
        public float HP           => _initialized ? _context.HP.Percentage : default;
        public float LifeSpan     => _initialized ? _context.LifeSpan.Percentage : default;
        public float BreedingRate => _initialized ? _context.BreedingRate.Percentage : default;
        public StateType State    => _initialized ? _currentState.Type : StateType.Base;
        public ActorType Type     => _initialized ? _context.Type : ActorType.None;
        public Sex Sex            => _initialized ? _context.Sex : Sex.Male;
        public int Score => _initialized ? _context.Score : default;
        // ���񂾏ꍇ(�v�[���ɕԋp����)�̃t���O
        public bool IsDead => _initialized ? _isDead : false;

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
            // �����ʒu�̃Z����ɋ���Ƃ���������������
            FieldManager.Instance.SetActorOnCell(transform.position, _context.Type);
            // �킩��₷���悤�ɖ��O�ɐ��ʂ𔽉f���Ă���
            name += _context.Sex == Sex.Male ? "��" : "��";

            OnSpawned?.Invoke(this);
            _initialized = true;
            MessageBroker.Default.Publish(new ActorSpawnMessage() { Type = _context.Type });
        }

        // ���S����ۂɔ�\���ɂȂ�
        void OnDisable()
        {
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
        public void StepAction()
        {
            _currentState = _currentState.Update();
        }

        /// <summary>
        /// �]���X�e�[�g�������ꍇ�Ƀ��Z�b�g���鏈��
        /// </summary>
        public void ResetOnEvaluateState()
        {
            // �q�����Y�ރR���[�`�����~
            if (_spawnChild != null) StopCoroutine(_spawnChild);
            _isMating = false;
        }

        /// <summary>
        /// �]���X�e�[�g�ȊO�ł͕]�����Ȃ����ƂŁA���t���[���]���������s���̂�h��
        /// ���g�̏��ƃ��[�_�[�̕]���l�����Ɏ��̍s�������߂�B
        /// </summary>
        public void Evaluate(float[] leaderEvaluate)
        {

            // �G�ɑ_���Ă���ꍇ�́A�U���������͓����邱�Ƃ��ŗD��Ȃ̂ŁA�]���l����ɔ��肷��
            SearchEnemy();

            // �]���l��p���Ď��̍s����I��
            ActionType action = _evaluator.SelectAction(leaderEvaluate);
            // ���S�͂��̂܂܎���
            if (action == ActionType.Killed) { _context.NextAction = ActionType.Killed; return; }
            else if (action == ActionType.Senility) { _context.NextAction = ActionType.Senility; return; }
            // �U��/������ꍇ�͌o�H���K�v
            else if (action == ActionType.Attack)
            {
                if (TryPathfindingToEnemy()) { _context.NextAction = ActionType.Attack; return; }
                else _context.Enemy = null; // ���̃X�e�[�g�ɑJ�ڂ���̂œG�ւ̎Q�Ƃ��폜
            }
            else if (action == ActionType.Escape)
            {
                if (TryPathfindingToEscapePoint()) { _context.NextAction = ActionType.Escape; return; }
                else _context.Enemy = null; // ���̃X�e�[�g�ɑJ�ڂ���̂œG�ւ̎Q�Ƃ��폜
            }
            // �ɐB����ꍇ�͗Y�Ǝ��Ŏ��s�����Ⴄ
            else if (action == ActionType.Breed)
            {
                if (_context.Sex == Sex.Male && TryDetectPartner()) { _context.NextAction = ActionType.Breed; return; }
                else if (_context.Sex == Sex.Female) { _context.NextAction = ActionType.Breed; return; }
            }
            // ���������͐H����T���ꍇ�A�Ώۂ̎����܂ł̌o�H���K�v
            else if (action == ActionType.SearchWater)
            {
                if (TryDetectResource(ResourceType.Water)) { _context.NextAction = ActionType.SearchWater; return; }
            }
            else if (action == ActionType.SearchFood)
            {
                if (TryDetectResource(ResourceType.Tree)) { _context.NextAction = ActionType.SearchFood; return; }
            }
            // �W��
            else if (action == ActionType.Gather)
            {
                if (TryPathfindingToGatherPoint()) { _context.NextAction = ActionType.Gather; return; }
            }

            // �����_���ɗׂ̃Z���Ɉړ�����
            _context.NextAction = ActionType.Wander;
        }

        /// <summary>
        /// ���E���̓G��T��
        /// �������m�����ꍇ�͈�Ԏ�߂��G��ΏۂƂ���
        /// </summary>
        void SearchEnemy()
        {
            Array.Clear(_detected, 0, _detected.Length);

            Vector3 pos = transform.position;
            float radius = _context.Base.SightRadius;
            LayerMask layer = _context.Base.SightTargetLayer;
            if (Physics.OverlapSphereNonAlloc(pos, radius, _detected, layer) == 0) return;

            // �߂����ɔz��ɓ����Ă���̂ŁA��ԋ߂��G��Ώۂ̓G�Ƃ��ď������ށB
            foreach (Collider collider in _detected)
            {
                if (collider == null) break;
                if (collider.CompareTag(_context.EnemyTag))
                {
                    collider.TryGetComponent(out _context.Enemy);
                    break;
                }
            }
        }

        /// <summary>
        /// �G�܂ł̌o�H��T������
        /// </summary>
        /// <returns>�o�H����:true �Ȃ�:false</returns>
        bool TryPathfindingToEnemy()
        {
            DeletePath();

            // �O���b�h��ŋ�����r
            Vector3 pos = transform.position;
            Vector3 enemyPos = _context.Enemy.transform.position;
            Vector2Int currentIndex = FieldManager.Instance.WorldPosToGridIndex(pos);
            Vector2Int enemyIndex = FieldManager.Instance.WorldPosToGridIndex(enemyPos);
            int dx = Mathf.Abs(currentIndex.x - enemyIndex.x);
            int dy = Mathf.Abs(currentIndex.y - enemyIndex.y);
            if (dx <= 1 && dy <= 1)
            {
                // �ׂ̃Z���ɂ���ꍇ�͈ړ����Ȃ��̂ŁA���ݒn���o�H�Ƃ��Ēǉ�����
                _context.Path.Add(pos);
                FieldManager.Instance.SetActorOnCell(pos, _context.Type);
                return true;
            }
            else
            {
                // �Ώۂ̃Z�� + ���͔��ߖT�ɑ΂��Čo�H�T��
                foreach (Vector2Int dir in Utility.SelfAndEightDirections)
                {
                    Vector2Int dirIndex = enemyIndex + dir;
                    // �o�H��������Ȃ������ꍇ�͒e��
                    if (!FieldManager.Instance.TryGetPath(currentIndex, dirIndex, out _context.Path)) continue;
                    // �o�H�̖��[(�G�̃Z���̗�)�ɃL�����N�^�[������ꍇ�͒e��
                    int goal = _context.Path.Count - 1;
                    if (FieldManager.Instance.IsActorOnCell(_context.Path[goal], out ActorType _)) continue;

                    FieldManager.Instance.SetActorOnCell(_context.Path[goal], _context.Type);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// ������o�H��T������
        /// </summary>
        /// <returns>�o�H����:true �Ȃ�:false</returns>
        bool TryPathfindingToEscapePoint()
        {
            DeletePath();

            // �O���b�h��ŋ�����r
            Vector3 pos = transform.position;
            Vector3 enemyPos = _context.Enemy.transform.position;
            Vector3 dir = (pos - enemyPos).normalized;
            Vector2Int currentIndex = FieldManager.Instance.WorldPosToGridIndex(pos);
            for (int i = 10; i >= 1; i--) // �K���Ȓl
            {
                Vector3 escapePos = dir * i;
                Vector2Int escapeIndex = FieldManager.Instance.WorldPosToGridIndex(escapePos);
                int dx = Mathf.Abs(currentIndex.x - escapeIndex.x);
                int dy = Mathf.Abs(currentIndex.y - escapeIndex.y);
                if (dx <= 1 && dy <= 1)
                {
                    // �ׂ̃Z���ɂ���ꍇ�͈ړ����Ȃ��̂ŁA���ݒn���o�H�Ƃ��Ēǉ�����
                    _context.Path.Add(pos);
                    FieldManager.Instance.SetActorOnCell(pos, _context.Type);
                    return true;
                }
                else
                {
                    // �o�H��������Ȃ������ꍇ�͒e��
                    if (!FieldManager.Instance.TryGetPath(currentIndex, escapeIndex, out _context.Path)) continue;
                    // �o�H�̖��[(�G�̃Z���̗�)�ɃL�����N�^�[������ꍇ�͒e��
                    int goal = _context.Path.Count - 1;
                    if (FieldManager.Instance.IsActorOnCell(_context.Path[goal], out ActorType _)) continue;

                    FieldManager.Instance.SetActorOnCell(_context.Path[goal], _context.Type);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// ���݂̌o�H���폜(��ɂ���)���A���[�̗\����폜����B
        /// �o�H��T������ۂɌĂ΂Ȃ��ƈȑO�̌o�H�̖��[�̗\�񂪎c�����܂܂ɂȂ��Ă��܂��B
        /// </summary>
        void DeletePath()
        {
            if (_context.Path.Count > 0)
            {
                int goal = _context.Path.Count - 1;
                FieldManager.Instance.SetActorOnCell(_context.Path[goal], ActorType.None);
                _context.Path.Clear();
            }
        }

        /// <summary>
        /// �������m���A�o�H�T�����s���B�o�H�����������ꍇ�̓S�[���̃Z����\�񂷂�B
        /// ���̃Z�� + ���͔��ߖT �̃Z���ւ̌o�H�����݂��邩���ׂ�
        /// </summary>
        /// <returns>���ւ̌o�H������:true �������Ȃ�of���ւ̌o�H������:false</returns>
        public bool TryDetectPartner()
        {
            Array.Clear(_detected, 0, _detected.Length);
            DeletePath();

            Vector3 pos = transform.position;
            float radius = _context.Base.SightRadius;
            LayerMask layer = _context.Base.SightTargetLayer;
            if (Physics.OverlapSphereNonAlloc(pos, radius, _detected, layer) == 0) return false;

            foreach (Collider collider in _detected)
            {
                if (collider == null) break;
                if (collider.transform == transform) continue; // ������e��
                // ���ȊO��e��
                if (collider.TryGetComponent(out DataContext target) && target.Sex == Sex.Female)
                {
                    // �O���b�h��ŋ�����r
                    Vector2Int currentIndex = FieldManager.Instance.WorldPosToGridIndex(pos);
                    Vector2Int targetIndex = FieldManager.Instance.WorldPosToGridIndex(target.Transform.position);
                    int dx = Mathf.Abs(currentIndex.x - targetIndex.x);
                    int dy = Mathf.Abs(currentIndex.y - targetIndex.y);
                    if (dx <= 1 && dy <= 1)
                    {
                        // �ׂ̃Z���ɂ���ꍇ�͈ړ����Ȃ��̂ŁA���ݒn���o�H�Ƃ��Ēǉ�����
                        _context.Path.Add(pos);
                        FieldManager.Instance.SetActorOnCell(pos, _context.Type);
                        return true;
                    }
                    else
                    {
                        // �Ώۂ̃Z�� + ���͔��ߖT�ɑ΂��Čo�H�T��
                        foreach (Vector2Int dir in Utility.SelfAndEightDirections)
                        {
                            Vector2Int dirIndex = targetIndex + dir;
                            // �o�H��������Ȃ������ꍇ�͒e��
                            if (!FieldManager.Instance.TryGetPath(currentIndex, dirIndex, out _context.Path)) continue;
                            // �o�H�̖��[(�����̃Z���̗�)�ɃL�����N�^�[������ꍇ�͒e��
                            int goal = _context.Path.Count - 1;
                            if (FieldManager.Instance.IsActorOnCell(_context.Path[goal], out ActorType _)) continue;

                            FieldManager.Instance.SetActorOnCell(_context.Path[goal], _context.Type);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// �����܂ł̌o�H�T��
        /// �o�H�����������ꍇ�̓S�[���̃Z����\�񂷂�
        /// </summary>
        /// <returns>�o�H����:true �o�H����:false</returns>
        bool TryDetectResource(ResourceType resource)
        {
            DeletePath();

            // �H���̃Z�������邩���ׂ�
            if (FieldManager.Instance.TryGetResourceCells(resource, out List<Cell> cellList))
            {
                // �H���̃Z�����߂����Ɍo�H�T��
                Vector3 pos = transform.position;
                foreach (Cell food in cellList.OrderBy(c => Vector3.SqrMagnitude(c.Pos - pos)))
                {
                    // TODO:�S�Ă̐H���ɑ΂��Čo�H�T��������Əd���̂ł�����x�̏��őł��؂鏈��

                    Vector2Int currentIndex = FieldManager.Instance.WorldPosToGridIndex(pos);
                    Vector2Int foodIndex = FieldManager.Instance.WorldPosToGridIndex(food.Pos);

                    int dx = Mathf.Abs(currentIndex.x - foodIndex.x);
                    int dy = Mathf.Abs(currentIndex.y - foodIndex.y);
                    if (dx <= 1 && dy <= 1)
                    {
                        // �ׂ̃Z���ɐH��������ꍇ�͈ړ����Ȃ��̂ŁA���ݒn���o�H�Ƃ��Ēǉ�����
                        _context.Path.Add(pos);
                        FieldManager.Instance.SetActorOnCell(pos, _context.Type);
                        return true;
                    }
                    else
                    {
                        // �Ώۂ̃Z�� + ���͔��ߖT�ɑ΂��Čo�H�T��
                        foreach (Vector2Int dir in Utility.SelfAndEightDirections)
                        {
                            Vector2Int targetIndex = foodIndex + dir;
                            // �o�H��������Ȃ������ꍇ�͒e��
                            if (!FieldManager.Instance.TryGetPath(currentIndex, targetIndex, out _context.Path)) continue;
                            // �o�H�̖��[(�����̃Z���̗�)�Ɏ����L�����N�^�[������ꍇ�͒e��
                            int goal = _context.Path.Count - 1;
                            FieldManager.Instance.TryGetCell(_context.Path[goal], out Cell cell);
                            if (!cell.IsEmpty) continue;

                            FieldManager.Instance.SetActorOnCell(_context.Path[goal], _context.Type);
                            return true;
                        }
                    }
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// �Y�������Ăяo���B
        /// ������łȂ��ꍇ�A��莞�Ԍo�ߌ�A�q�����Y�ޏ������Ăяo���B
        /// �L�����Z�����邱�Ƃ��o����B
        /// </summary>
        public void SpawnChild(uint maleGene, UnityAction callback = null)
        {           
            if (_currentState.Type != StateType.FemaleBreed)
            {
                Debug.LogWarning($"{name} ���̔ɐB�X�e�[�g�ȊO�Ŏq�����Y�ޏ������Ă΂�Ă���B");
            }

            if (!_isMating)
            {
                _isMating = true;
                _spawnChild = StartCoroutine(SpawnChildCoroutine(maleGene, callback));
            }
        }

        IEnumerator SpawnChildCoroutine(uint maleGene, UnityAction callback = null)
        {
            // ���o�Ƃ��ăp�[�e�B�N�������񂩏o��
            int c = 3; // �J��Ԃ��񐔂͓K��
            for (int i = 0; i < c; i++) 
            {
                MessageBroker.Default.Publish(new PlayParticleMessage()
                {
                    Type = ParticleType.Mating,
                    Pos = transform.position,
                });
                yield return new WaitForSeconds(_context.Base.MatingTime / c);
            }
            // ���͔��ߖT�̃Z���Ɏq�����Y��
            if (TryGetNeighbourPos(out Vector3 pos))
            {
                _spawnChildMessage.Gene1 = maleGene;
                _spawnChildMessage.Gene2 = _context.Gene;
                _spawnChildMessage.Food = _context.Food.Value;
                _spawnChildMessage.Water = _context.Water.Value;
                _spawnChildMessage.HP = _context.HP.Value;
                _spawnChildMessage.LifeSpan = _context.LifeSpan.Value;
                _spawnChildMessage.Pos = pos;
                MessageBroker.Default.Publish(_spawnChildMessage);
                // �q�����Y�񂾂̂ŔɐB����0�ɂ���
                _context.BreedingRate.Value = 0;
            }

            yield return null;
            _isMating = false;
            callback?.Invoke();
        }

        /// <summary>
        /// ���͔��ߖT�̃Z���𒲂ׁA�q�𐶐�����ʒu���擾����
        /// </summary>
        /// <returns>�擾�o����:true ��������Z��������:false</returns>
        bool TryGetNeighbourPos(out Vector3 pos)
        {
            Vector2Int index = FieldManager.Instance.WorldPosToGridIndex(transform.position);
            foreach (Vector2Int dir in Utility.EightDirections)
            {
                Vector2Int neighbourIndex = index + dir;
                // ���n�������������A�L�����N�^�[�����Ȃ��Z��
                if (!FieldManager.Instance.IsWithInGrid(neighbourIndex)) continue;
                if (!FieldManager.Instance.TryGetCell(neighbourIndex, out Cell cell)) continue;
                if (!cell.IsWalkable) continue;
                if (!cell.IsEmpty) continue;

                // �������鍂�������g�̍����ɍ��킹��
                pos = cell.Pos;
                pos.y = transform.position.y;

                return true;
            }

            pos = Vector3.zero;
            return false;
        }

        /// <summary>
        /// �W���n�_�ւ̌o�H��T������
        /// �W���n�_����X�p�C�����ɒT�����Ă����B
        /// </summary>
        /// <returns>�W���n�_�ւ̌o�H������:true �W���n�_�ւ̌o�H������:false</returns>
        bool TryPathfindingToGatherPoint()
        {
            DeletePath();
            
            Vector2Int currentIndex = FieldManager.Instance.WorldPosToGridIndex(transform.position);
            Vector2Int gatherIndex = FieldManager.Instance.WorldPosToGridIndex(PublicBlackBoard.GatherPos);

            int dx = Mathf.Abs(currentIndex.x - gatherIndex.x);
            int dy = Mathf.Abs(currentIndex.y - gatherIndex.y);
            if (dx <= 1 && dy <= 1)
            {
                // �ׂ̃Z���ɐH��������ꍇ�͈ړ����Ȃ��̂ŁA���ݒn���o�H�Ƃ��Ēǉ�����
                _context.Path.Add(transform.position);
                FieldManager.Instance.SetActorOnCell(transform.position, _context.Type);
                return true;
            }
            else
            {
                Vector2Int[] dirs =
                {
                    Vector2Int.right, Vector2Int.up, Vector2Int.left, Vector2Int.down,
                };

                int count = 0;
                int sideLength = 1;
                int sideCount = 0;
                // �L�����N�^�[�̍ő吔���J��Ԃ�
                while (count++ < InvalidActorHolder.PoolSize)
                {
                    int index = sideCount % 4;
                    for (int k = 0; k < sideLength; k++)
                    {
                        gatherIndex.y += dirs[index].y;
                        gatherIndex.x += dirs[index].x;

                        // �o�H�����݂��邩�H
                        if (!FieldManager.Instance.TryGetPath(currentIndex, gatherIndex, out _context.Path)) continue;
                        // �o�H�̖��[(�����̃Z���̗�)�Ɏ����L�����N�^�[������ꍇ�͒e��
                        int goal = _context.Path.Count - 1;
                        // TODO:�o�H�̒�����0�̏ꍇ������
                        if (goal < 0) continue;
                        // �o�H�̖��[�̃Z�����󂩂ǂ���
                        FieldManager.Instance.TryGetCell(_context.Path[goal], out Cell cell);
                        if (!cell.IsEmpty) continue;

                        FieldManager.Instance.SetActorOnCell(_context.Path[goal], _context.Type);
                        return true;
                    }

                    // 2�ӈړ��������ӓ�����̒�����1������
                    if (index == 1 || index == 3) sideLength++;

                    sideCount++;
                }

                return false;
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