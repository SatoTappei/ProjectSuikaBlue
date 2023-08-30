using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class FieldManager : MonoBehaviour
    {
        // �V���O���g��
        public static FieldManager Instance { get; private set; }

        [SerializeField] FieldBuilder _builder;
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

        /// <summary>
        /// �����ɑΉ������Z����S�ĕԂ��B�Z���������ꍇ�̓��X�g���쐬���Ԃ��̂�null��Ԃ����Ƃ������B
        /// </summary>
        /// <returns>�Z����1�ȏ�:true 0��:false</returns>
        public bool TryGetResourceCells(ResourceType type, out List<Cell> list)
        {
            ThrowIfFieldIsNull();

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
            ThrowIfFieldIsNull();

            Vector2Int index = WorldPosToGridIndex(pos);
            if(FieldUtility.IsWithinGrid(_field, index))
            {
                cell = _field[index.y, index.x];
                return true;
            }
            else
            {
                cell = null;
                return false;
            }
        }

        public bool TryGetCell(Vector2Int index, out Cell cell)
        {
            ThrowIfFieldIsNull();
            
            bool isWithin = FieldUtility.IsWithinGrid(_field, index);
            cell = isWithin ? _field[index.y, index.x] : null;            
            return isWithin;
        }

        /// <summary>
        /// �J�n�n�_�������͖ړI�n�ɃO���b�h�O���w�肵���ꍇ��Path��null�ɂȂ�B
        /// �ړI�n�ɂ��ǂ蒅���Ȃ������ꍇ�͏�Q���̎�O�܂ł�Path�ɂȂ�B
        /// </summary>
        /// <returns>�ړI�n�ɂ��ǂ蒅����:true ��Q���ɂԂ�����/�O���b�h�O:false</returns>
        public bool TryGetPath(in Vector3 startPos, in Vector3 goalPos, out Stack<Vector3> path)
        {
            Vector2Int startIndex = WorldPosToGridIndex(startPos);
            Vector2Int goalIndex = WorldPosToGridIndex(goalPos);
            return TryGetPath(startIndex, goalIndex, out path);
        }

        /// <summary>
        /// �J�n�n�_�������͖ړI�n�ɃO���b�h�O���w�肵���ꍇ��Path��null�ɂȂ�B
        /// �ړI�n�ɂ��ǂ蒅���Ȃ������ꍇ�͏�Q���̎�O�܂ł�Path�ɂȂ�B
        /// </summary>
        /// <returns>�ړI�n�ɂ��ǂ蒅����:true ��Q���ɂԂ�����/�O���b�h�O:false</returns>
        public bool TryGetPath(Vector2Int startIndex, Vector2Int goalIndex, out Stack<Vector3> path)
        {
            bool hasStart = FieldUtility.IsWithinGrid(_field, startIndex);
            bool hasGoal = FieldUtility.IsWithinGrid(_field, goalIndex);

            if (hasStart && hasGoal)
            {
                bool isGoal = _bresenham.TryGetPath(startIndex, goalIndex, out Stack<Vector2Int> indexes);
                path = CreatePath(indexes);

                return isGoal;
            }
            else
            {
                path = null;
                return false;
            }
        }

        /// <summary>
        /// �Y�����ɑΉ������Z���̈ʒu��Stack�ɋl�߂Ă���
        /// </summary>
        /// <returns>�ʂ�Z���̈ʒu�̃X�^�b�N</returns>
        Stack<Vector3> CreatePath(Stack<Vector2Int> indexes)
        {
            Stack<Vector3> path = new(indexes.Count);
            foreach (Vector2Int index in indexes)
            {
                path.Push(_field[index.y, index.x].Pos);
            }

            return path;
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