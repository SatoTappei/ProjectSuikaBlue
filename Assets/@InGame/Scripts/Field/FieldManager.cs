using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class FieldManager : MonoBehaviour
    {
        // シングルトン
        public static FieldManager Instance { get; private set; }

        [SerializeField] FieldBuilder _builder;
        [SerializeField] ResourceDataHolder _dataHolder;
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

        public int GetResourceHealingLimit(ResourceType type) => _dataHolder.GetHealingLimit(type);
        public bool IsWithInGrid(Vector2Int index)=> FieldUtility.IsWithinGrid(_field, index);

        /// <summary>
        /// キャラクターが存在するという情報をセルにセットする
        /// 情報を消したい場合は第二引数にNoneを指定する
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
        /// 指定した座標のセルにキャラクターが存在するかどうかを判定する
        /// </summary>
        /// <returns>存在する:true 存在しない:false</returns>
        public bool IsActorOnCell(in Vector3 pos, out ActorType type)
        {
            TryGetCell(pos, out Cell cell);
            type = cell.ActorType;
            return cell.ActorType != ActorType.None;
        }

        public bool IsActorOnCell(Vector2Int index, out ActorType type)
        {
            // TryGetCellメソッドはVector3をVector2Intに変換しているのでそれを省略する
            if (FieldUtility.IsWithinGrid(_field, index))
            {
                Cell cell = _field[index.y, index.x];
                type = cell.ActorType;
                return cell.ActorType != ActorType.None;
            }
            else
            {
                Debug.LogWarning("グリッド外を指定している: " + index);
                type = ActorType.None;
                return false;
            }
        }

        /// <summary>
        /// 資源に対応したセルを全て返す。セルが無い場合はリストを作成し返すのでnullを返すことが無い。
        /// </summary>
        /// <returns>セルが1個以上:true 0個:false</returns>
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
                throw new KeyNotFoundException("この資源のセルが登録されていない: " + type);
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
                Debug.LogWarning("グリッド外を指定している: " + index);
                cell = null;
                return false;
            }
        }

        /// <summary>
        /// 開始地点もしくは目的地にグリッド外を指定した場合はPathがnullになる。
        /// 目的地にたどり着かなかった場合は障害物の手前までのPathになる。
        /// </summary>
        /// <returns>目的地にたどり着いた:true 障害物にぶつかった/グリッド外:false</returns>
        public bool TryGetPath(in Vector3 startPos, in Vector3 goalPos, out List<Vector3> path)
        {
            Vector2Int startIndex = WorldPosToGridIndex(startPos);
            Vector2Int goalIndex = WorldPosToGridIndex(goalPos);
            return TryGetPath(startIndex, goalIndex, out path);
        }

        /// <summary>
        /// 目的地にたどり着かなかった場合は障害物の手前までのPathになる。
        /// </summary>
        /// <returns>目的地にたどり着いた:true 障害物にぶつかった/グリッド外:false</returns>
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
                path = new(); // TODO:経路の両端がグリッド内に無い場合はnewしている。
                return false;
            }
        }

        /// <summary>
        /// 添え字に対応したセルの位置を経路に詰めていく
        /// </summary>
        /// <returns>通るセルの位置の経路</returns>
        List<Vector3> CreatePath(List<Vector2Int> indexes)
        {     
            List<Vector3> path = new(indexes.Count); // TODO: 経路を作る度にnewしている
            foreach (Vector2Int index in indexes)
            {
                path.Add(_field[index.y, index.x].Pos);
            }

            return path;
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