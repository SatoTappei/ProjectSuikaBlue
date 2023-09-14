using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class FieldManager : MonoBehaviour
    {
        // シングルトン
        public static FieldManager Instance { get; private set; }

        [SerializeField] FieldBuilder _builder;
        [SerializeField] ResourceDataHolder _resourceDataHolder;
        [Header("Startのタイミングで生成する")]
        [SerializeField] bool _buildOnStart;
        [Header("デバッグ用:セルの状態を描画する")]
        [SerializeField] bool _isDebug = false;

        Cell[,] _field;
        Dictionary<ResourceType, List<Cell>> _resourceCellDict = new();
        Bresenham _bresenham;

        public ResourceDataHolder Resource => _resourceDataHolder;

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

        void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (_isDebug && _field != null) DebugDrawCellStatus();
#endif
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

        public bool IsWithInGrid(in Vector3 pos)
        {
            Vector2Int index = WorldPosToGridIndex(pos);
            return IsWithinGrid(index);
        }

        public bool IsWithinGrid(Vector2Int index)
        {
            int y = index.y;
            int x = index.x;
            return 0 <= y && y < _field.GetLength(0) && 0 <= x && x < _field.GetLength(1);
        }

        /// <summary>
        /// キャラクターが存在するという情報をセルにセットする
        /// 情報を消したい場合は第二引数にNoneを指定する
        /// </summary>
        public void SetActorOnCell(in Vector3 pos, ActorType type)
        {
            TryGetCell(pos, out Cell cell);
            cell.ActorType = type;
        }

        public void SetActorOnCell(Vector2Int index, ActorType type)
        {
            TryGetCell(index, out Cell cell);
            cell.ActorType = type;
        }

        public void DeleteActorOnCell(in Vector3 pos) => SetActorOnCell(pos, ActorType.None);
        public void DeleteActorOnCell(Vector2Int index) => SetActorOnCell(index, ActorType.None);

        /// <summary>
        /// 指定した座標のセルにキャラクターが存在するかどうかを判定する
        /// </summary>
        /// <returns>存在する:true 存在しない:false</returns>
        public bool IsActorOnCell(in Vector3 pos, out ActorType type)
        {
            TryGetCell(pos, out Cell cell);
            type = cell.ActorType;
            return type != ActorType.None;
        }

        public bool IsActorOnCell(Vector2Int index, out ActorType type)
        {
            // TryGetCellメソッドはVector3をVector2Intに変換しているのでそれを省略する
            if (IsWithinGrid(index))
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
        /// 資源に対応したセルを全て返す。セルが無い場合は空のリストを返すのでnullを返すことが無い。
        /// </summary>
        /// <returns>セルが1個以上:true 0個:false</returns>
        public bool TryGetResourceCells(ResourceType type, out List<Cell> list)
        {
            if (_resourceCellDict.ContainsKey(type))
            {
                list = _resourceCellDict[type];
                list ??= new(); // TODO:資源のセルが無い場合にnewしている
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
            if (IsWithinGrid(index))
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
        public bool TryGetPath(in Vector3 startPos, in Vector3 goalPos, ref List<Vector3> path)
        {
            Vector2Int startIndex = WorldPosToGridIndex(startPos);
            Vector2Int goalIndex = WorldPosToGridIndex(goalPos);
            return TryGetPath(startIndex, goalIndex, ref path);
        }

        /// <summary>
        /// Pathの中身をクリアし、経路を詰めていく。
        /// 目的地にたどり着かなかった場合は障害物の手前までのPathになる。
        /// </summary>
        /// <returns>目的地にたどり着いた:true 障害物にぶつかった/グリッド外:false</returns>
        public bool TryGetPath(Vector2Int startIndex, Vector2Int goalIndex, ref List<Vector3> path)
        {
            path.Clear();

            bool hasStart = IsWithinGrid(startIndex);
            bool hasGoal = IsWithinGrid(goalIndex);

            if (hasStart && hasGoal)
            {
                bool isGoal = _bresenham.TryGetPath(startIndex, goalIndex, out List<Vector2Int> indexes);
                // 経路を詰めていく
                foreach (Vector2Int index in indexes)
                {
                    path.Add(_field[index.y, index.x].Pos);
                }

                return isGoal;
            }
            else
            {
                path = new(); // TODO:経路の両端がグリッド内に無い場合はnewしている。
                return false;
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

        void DebugDrawCellStatus()
        {
            foreach (Cell cell in _field)
            {
                Gizmos.color = cell.IsEmpty ? Color.blue : Color.red;
                Gizmos.DrawCube(cell.Pos, new Vector3(0.9f, 1.1f, 0.9f));
            }
        }
    }
}