using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class FieldManager : MonoBehaviour
    {
        // シングルトン
        public static FieldManager Instance { get; private set; }

        [SerializeField] FieldBuilder _builder;
        [Header("Startのタイミングで生成する")]
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
        /// 資源に対応したセルを全て返す。セルが無い場合はリストを作成し返すのでnullを返すことが無い。
        /// </summary>
        /// <returns>セルが1個以上:true 0個:false</returns>
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
                throw new KeyNotFoundException("この資源のセルが登録されていない: " + type);
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
        /// 開始地点もしくは目的地にグリッド外を指定した場合はPathがnullになる。
        /// 目的地にたどり着かなかった場合は障害物の手前までのPathになる。
        /// </summary>
        /// <returns>目的地にたどり着いた:true 障害物にぶつかった/グリッド外:false</returns>
        public bool TryGetPath(in Vector3 startPos, in Vector3 goalPos, out Stack<Vector3> path)
        {
            Vector2Int startIndex = WorldPosToGridIndex(startPos);
            Vector2Int goalIndex = WorldPosToGridIndex(goalPos);
            return TryGetPath(startIndex, goalIndex, out path);
        }

        /// <summary>
        /// 開始地点もしくは目的地にグリッド外を指定した場合はPathがnullになる。
        /// 目的地にたどり着かなかった場合は障害物の手前までのPathになる。
        /// </summary>
        /// <returns>目的地にたどり着いた:true 障害物にぶつかった/グリッド外:false</returns>
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
        /// 添え字に対応したセルの位置をStackに詰めていく
        /// </summary>
        /// <returns>通るセルの位置のスタック</returns>
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
        /// 外部からFieldの生成前にセルの情報や経路を探索しようとしてるケースを検知する
        /// </summary>
        void ThrowIfFieldIsNull()
        {
            if (_field == null)
            {
                string msg = "Fieldが未生成の状態でセルの情報もしくは経路を取得しようとしている";
                throw new System.NullReferenceException(msg);
            }
        }

        /// <summary>
        /// ワールド座標に対応したグリッドの添え字を返す
        /// Y座標を無視して計算する
        /// </summary>
        public Vector2Int WorldPosToGridIndex(in Vector3 pos)
        {
            // グリッドの前後左右
            float forwardZ = _field[0, 0].Pos.z;
            float backZ = _field[_field.GetLength(0) - 1, 0].Pos.z;
            float leftX = _field[0, 0].Pos.x;
            float rightX = _field[0, _field.GetLength(1) - 1].Pos.x;
            // グリッドの1辺
            float width = rightX - leftX;
            float height = backZ - forwardZ;
            // グリッドの端からの距離
            float fromPosZ = pos.z - forwardZ;
            float fromPosX = pos.x - leftX;
            // グリッドの橋から何％の位置か
            float percentZ = Mathf.Abs(fromPosZ / height);
            float percentX = Mathf.Abs(fromPosX / width);

            // xはそのまま、yはzに対応している
            Vector2Int index = new Vector2Int()
            {
                x = Mathf.RoundToInt((_field.GetLength(1) - 1) * percentX),
                y = Mathf.RoundToInt((_field.GetLength(0) - 1) * percentZ),
            };

            return index;
        }

        /// <summary>
        /// 外部から必要に応じてこのクラスのインスタンスが存在するかどうか調べる
        /// </summary>
        public static void CheckInstance()
        {
            if (Instance == null)
            {
                Debug.LogError("FieldManagerのインスタンスが存在しない");
            }
        }
    }
}