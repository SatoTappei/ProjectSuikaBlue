using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class FieldManager : MonoBehaviour
    {
        // �V���O���g��
        public static FieldManager Instance { get; private set; }

        [SerializeField] FieldBuilder _builder;
        [SerializeField] ResourceDataHolder _dataHolder;
        [Header("Start�̃^�C�~���O�Ő�������")]
        [SerializeField] bool _buildOnStart;

        Cell[,] _field;
        Dictionary<ResourceType, List<Cell>> _resourceCellDict = new();
        Bresenham _bresenham;

        public Cell[,] Field
        {
            get
            {
                _field ??= _builder.Build();
                return _field;
            }
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            if (_buildOnStart && _field == null)
            {
                Create();
            }
        }

        void OnDestroy()
        {
            _field = null;
            _resourceCellDict = null;
            _bresenham = null;
            Instance = null;
        }

        public Cell[,] Create()
        {
            _field = _builder.Build();
            CategorizeCell(_field);
            _bresenham = new(_field);

            return _field;
        }

        void CategorizeCell(Cell[,] field)
        {
            foreach(Cell cell in field)
            {
                if (cell.ResourceType == ResourceType.None) continue;

                if (!_resourceCellDict.ContainsKey(cell.ResourceType))
                {
                    _resourceCellDict.Add(cell.ResourceType, new());
                }
                
                _resourceCellDict[cell.ResourceType].Add(cell);
            }
        }

        public int GetResourceHealingLimit(ResourceType type) => _dataHolder.GetHealingLimit(type);
        public bool IsWithInGrid(Vector2Int index)=> FieldUtility.IsWithinGrid(_field, index);

        /// <summary>
        /// �L�����N�^�[�����݂���Ƃ��������Z���ɃZ�b�g����
        /// �������������ꍇ�͑�������None���w�肷��
        /// </summary>
        public void SetActorOnCell(in Vector3 pos, ActorType type)
        {
            TryGetCell(pos, out Cell cell);
            cell.ActorType = type;
        }

        public void SetActorOnCell(in Vector2Int index, ActorType type)
        {
            TryGetCell(index, out Cell cell);
            cell.ActorType = type;
        }

        /// <summary>
        /// �w�肵�����W�̃Z���ɃL�����N�^�[�����݂��邩�ǂ����𔻒肷��
        /// </summary>
        /// <returns>���݂���:true ���݂��Ȃ�:false</returns>
        public bool IsActorOnCell(in Vector3 pos, out ActorType type)
        {
            TryGetCell(pos, out Cell cell);
            type = cell.ActorType;
            return cell.ActorType != ActorType.None;
        }

        public bool IsActorOnCell(Vector2Int index, out ActorType type)
        {
            // TryGetCell���\�b�h��Vector3��Vector2Int�ɕϊ����Ă���̂ł�����ȗ�����
            if (FieldUtility.IsWithinGrid(_field, index))
            {
                Cell cell = _field[index.y, index.x];
                type = cell.ActorType;
                return cell.ActorType != ActorType.None;
            }
            else
            {
                Debug.LogWarning("�O���b�h�O���w�肵�Ă���: " + index);
                type = ActorType.None;
                return false;
            }
        }

        /// <summary>
        /// �����ɑΉ������Z����S�ĕԂ��B�Z���������ꍇ�̓��X�g���쐬���Ԃ��̂�null��Ԃ����Ƃ������B
        /// </summary>
        /// <returns>�Z����1�ȏ�:true 0��:false</returns>
        public bool TryGetResourceCells(ResourceType type, out List<Cell> list)
        {
            if (_resourceCellDict.ContainsKey(type))
            {
                list = _resourceCellDict[type];
                list ??= new();
                return list.Count > 0;
            }
            else
            {
                throw new KeyNotFoundException("���̎����̃Z�����o�^����Ă��Ȃ�: " + type);
            }
        }

        public bool TryGetCell(in Vector3 pos, out Cell cell)
        {
            Vector2Int index = WorldPosToGridIndex(pos);
            return TryGetCell(index, out cell);
        }

        public bool TryGetCell(Vector2Int index, out Cell cell)
        {
            if (FieldUtility.IsWithinGrid(_field, index))
            {
                cell = _field[index.y, index.x];
                return true;
            }
            else
            {
                Debug.LogWarning("�O���b�h�O���w�肵�Ă���: " + index);
                cell = null;
                return false;
            }
        }

        /// <summary>
        /// �J�n�n�_�������͖ړI�n�ɃO���b�h�O���w�肵���ꍇ��Path��null�ɂȂ�B
        /// �ړI�n�ɂ��ǂ蒅���Ȃ������ꍇ�͏�Q���̎�O�܂ł�Path�ɂȂ�B
        /// </summary>
        /// <returns>�ړI�n�ɂ��ǂ蒅����:true ��Q���ɂԂ�����/�O���b�h�O:false</returns>
        public bool TryGetPath(in Vector3 startPos, in Vector3 goalPos, out List<Vector3> path)
        {
            Vector2Int startIndex = WorldPosToGridIndex(startPos);
            Vector2Int goalIndex = WorldPosToGridIndex(goalPos);
            return TryGetPath(startIndex, goalIndex, out path);
        }

        /// <summary>
        /// �ړI�n�ɂ��ǂ蒅���Ȃ������ꍇ�͏�Q���̎�O�܂ł�Path�ɂȂ�B
        /// </summary>
        /// <returns>�ړI�n�ɂ��ǂ蒅����:true ��Q���ɂԂ�����/�O���b�h�O:false</returns>
        public bool TryGetPath(Vector2Int startIndex, Vector2Int goalIndex, out List<Vector3> path)
        {
            bool hasStart = FieldUtility.IsWithinGrid(_field, startIndex);
            bool hasGoal = FieldUtility.IsWithinGrid(_field, goalIndex);

            if (hasStart && hasGoal)
            {
                bool isGoal = _bresenham.TryGetPath(startIndex, goalIndex, out List<Vector2Int> indexes);
                path = CreatePath(indexes);

                return isGoal;
            }
            else
            {
                path = new(); // TODO:�o�H�̗��[���O���b�h���ɖ����ꍇ��new���Ă���B
                return false;
            }
        }

        /// <summary>
        /// �Y�����ɑΉ������Z���̈ʒu���o�H�ɋl�߂Ă���
        /// </summary>
        /// <returns>�ʂ�Z���̈ʒu�̌o�H</returns>
        List<Vector3> CreatePath(List<Vector2Int> indexes)
        {     
            List<Vector3> path = new(indexes.Count); // TODO: �o�H�����x��new���Ă���
            foreach (Vector2Int index in indexes)
            {
                path.Add(_field[index.y, index.x].Pos);
            }

            return path;
        }

        /// <summary>
        /// ���[���h���W�ɑΉ������O���b�h�̓Y������Ԃ�
        /// Y���W�𖳎����Čv�Z����
        /// </summary>
        public Vector2Int WorldPosToGridIndex(in Vector3 pos)
        {
            // �O���b�h�̑O�㍶�E
            float forwardZ = _field[0, 0].Pos.z;
            float backZ = _field[_field.GetLength(0) - 1, 0].Pos.z;
            float leftX = _field[0, 0].Pos.x;
            float rightX = _field[0, _field.GetLength(1) - 1].Pos.x;
            // �O���b�h��1��
            float width = rightX - leftX;
            float height = backZ - forwardZ;
            // �O���b�h�̒[����̋���
            float fromPosZ = pos.z - forwardZ;
            float fromPosX = pos.x - leftX;
            // �O���b�h�̋����牽���̈ʒu��
            float percentZ = Mathf.Abs(fromPosZ / height);
            float percentX = Mathf.Abs(fromPosX / width);

            // x�͂��̂܂܁Ay��z�ɑΉ����Ă���
            Vector2Int index = new Vector2Int()
            {
                x = Mathf.RoundToInt((_field.GetLength(1) - 1) * percentX),
                y = Mathf.RoundToInt((_field.GetLength(0) - 1) * percentZ),
            };

            return index;
        }

        /// <summary>
        /// �O������Field�̐����O�ɃZ���̏���o�H��T�����悤�Ƃ��Ă�P�[�X�����m����
        /// </summary>
        void ThrowIfFieldIsNull()
        {
            if (_field == null)
            {
                string msg = "Field���������̏�ԂŃZ���̏��������͌o�H���擾���悤�Ƃ��Ă���";
                throw new System.NullReferenceException(msg);
            }
        }

        /// <summary>
        /// �O������K�v�ɉ����Ă��̃N���X�̃C���X�^���X�����݂��邩�ǂ������ׂ�
        /// </summary>
        public static void CheckInstance()
        {
            if (Instance == null)
            {
                Debug.LogError("FieldManager�̃C���X�^���X�����݂��Ȃ�");
            }
        }
    }
}