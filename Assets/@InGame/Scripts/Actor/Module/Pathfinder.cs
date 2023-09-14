using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class Pathfinder
    {
        DataContext _context;
        Collider[] _detected = new Collider[8];

        public Pathfinder(DataContext context)
        {
            _context = context;
        }

        Vector3 GoalPos => _context.Path[_context.Path.Count - 1];
        Vector3 Position => _context.Transform.position;

        /// <summary>
        /// 視界内の敵を探す
        /// 複数検知した場合は一番手近い敵を対象とする
        /// </summary>
        public bool SearchEnemy()
        {
            if (Detect.OverlapSphere(_context, _detected) == 0) return false;

            // 近い順に配列に入っているので、一番近い敵を対象の敵として書き込む。
            foreach (Collider collider in _detected)
            {
                if (collider != null && collider.CompareTag(_context.EnemyTag))
                {
                    // 敵をタグで判定、コンポーネントの取得
                    return collider.TryGetComponent(out _context.Enemy);
                }
            }

            return false;
        }

        /// <summary>
        /// 敵までの経路を探索する
        /// </summary>
        /// <returns>経路あり:true なし:false</returns>
        public bool TryPathfindingToEnemy()
        {
            DeletePath();

            // グリッド上で距離比較
            Vector3 enemyPos = _context.Enemy.transform.position;
            Vector2Int currentIndex = World2Grid(Position);
            Vector2Int enemyIndex = World2Grid(enemyPos);
            if (CreatePathIfNeighbourOnGrid(currentIndex, enemyIndex)) return true;

            // 対象のセル + 周囲八近傍に対して経路探索
            foreach (Vector2Int dir in Utility.SelfAndEightDirections)
            {
                Vector2Int dirIndex = enemyIndex + dir;
                // 経路が見つからなかった場合は弾く
                if (!TryGetPath(currentIndex, dirIndex)) continue;
                // 経路の末端(敵のセルの隣)にキャラクターがいる場合は弾く
                if (IsOnCell(GoalPos)) continue;

                SetOnCell(GoalPos);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 逃げる経路を探索する
        /// 経路の末端を周囲八近傍に一定の距離離れた位置から徐々に近づけていく
        /// </summary>
        /// <returns>経路あり:true なし:false</returns>
        public bool TryPathfindingToEscapePoint()
        {
            DeletePath();

            // グリッド上で距離比較
            Vector3 enemyPos = _context.Enemy.transform.position;
            Vector3 dir = (Position - enemyPos).normalized;
            Vector2Int currentIndex = World2Grid(Position);
            for (int i = 10; i >= 1; i--) // 適当な値
            {
                Vector3 escapePos = dir * i;
                Vector2Int escapeIndex = World2Grid(escapePos);
                if (CreatePathIfNeighbourOnGrid(currentIndex, escapeIndex)) return true;

                // 経路が見つからなかった場合は弾く
                if (!TryGetPath(currentIndex, escapeIndex)) continue;
                // 経路の末端(敵のセルの隣)にキャラクターがいる場合は弾く
                if (IsOnCell(GoalPos)) continue;

                SetOnCell(GoalPos);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 雌を検知し、経路探索を行う。経路が見つかった場合はゴールのセルを予約する。
        /// 雌のセル + 周囲八近傍 のセルへの経路が存在するか調べる
        /// </summary>
        /// <returns>雌への経路がある:true 雌がいないof雌への経路が無い:false</returns>
        public bool TryDetectPartner()
        {
            DeletePath();

            if (Detect.OverlapSphere(_context, _detected) == 0) return false;
            
            // nullと自身を弾く
            foreach (Collider collider in _detected.Where(c => c != null && c != _context.Transform))
            {
                if (collider.TryGetComponent(out DataContext target) && target.Sex == Sex.Female)
                {
                    // グリッド上で距離比較
                    Vector2Int currentIndex = World2Grid(Position);
                    Vector2Int targetIndex = World2Grid(target.Transform.position);
                    if (CreatePathIfNeighbourOnGrid(currentIndex, targetIndex)) return true;

                    // 対象のセル + 周囲八近傍に対して経路探索
                    foreach (Vector2Int dir in Utility.SelfAndEightDirections)
                    {
                        Vector2Int dirIndex = targetIndex + dir;
                        // 経路が見つからなかった場合は弾く
                        if (!TryGetPath(currentIndex, dirIndex)) continue;
                        // 経路の末端(資源のセルの隣)にキャラクターがいる場合は弾く
                        if (IsOnCell(GoalPos)) continue;

                        SetOnCell(GoalPos);
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 資源までの経路探索
        /// 経路が見つかった場合はゴールのセルを予約する
        /// </summary>
        /// <returns>経路あり:true 経路無し:false</returns>
        public bool TryPathfindingToResource(ResourceType resource)
        {
            DeletePath();

            // 資源のセルがあるか調べる
            if (!TryGetResourceCells(resource, out List<Cell> cellList)) return false;
            
            // 食料のセルを近い順に経路探索
            foreach (Cell food in cellList.OrderBy(c => Vector3.SqrMagnitude(c.Pos - Position)))
            {
                Vector2Int currentIndex = World2Grid(Position);
                Vector2Int resourceIndex = World2Grid(food.Pos);
                if (CreatePathIfNeighbourOnGrid(currentIndex, resourceIndex)) return true;

                // 対象のセル + 周囲八近傍に対して経路探索
                foreach (Vector2Int dir in Utility.SelfAndEightDirections)
                {
                    Vector2Int targetIndex = resourceIndex + dir;
                    // 経路が見つからなかった場合は弾く
                    if (!TryGetPath(currentIndex, targetIndex)) continue;
                    // 経路の末端(資源のセルの隣)に資源キャラクターがいる場合は弾く
                    if (IsOnCell(GoalPos)) continue;

                    SetOnCell(GoalPos);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 集合地点への経路を探索する
        /// 集合地点からスパイラルに探索していく。
        /// </summary>
        /// <returns>集合地点への経路がある:true 集合地点への経路が無い:false</returns>
        public bool TryPathfindingToGatherPoint()
        {
            DeletePath();

            Vector2Int currentIndex = World2Grid(Position);
            Vector2Int gatherIndex = World2Grid(PublicBlackBoard.GatherPos);
            if (CreatePathIfNeighbourOnGrid(currentIndex, gatherIndex)) return true;

            int count = 0;
            int sideLength = 1;
            int sideCount = 0;
            // キャラクターの最大数分繰り返す
            while (count++ < InvalidActorHolder.PoolSize)
            {
                int index = sideCount % 4;
                for (int k = 0; k < sideLength; k++)
                {
                    gatherIndex.y += Utility.Counterclockwise[index].y;
                    gatherIndex.x += Utility.Counterclockwise[index].x;

                    // 経路が存在するか？
                    if (!TryGetPath(currentIndex, gatherIndex)) continue;
                    // TODO:経路の長さが0の場合がある
                    if (_context.Path.Count == 0) continue;
                    // 経路の末端(資源のセルの隣)に資源キャラクターがいる場合は弾く
                    if (IsOnCell(GoalPos)) continue;

                    SetOnCell(GoalPos);
                    return true;
                }

                // 2辺移動したら一辺当たりの長さが1増える
                if (index == 1 || index == 3) sideLength++;

                sideCount++;
            }

            return false;
        }

        /// <summary>
        /// 現在の経路を削除(空にする)し、末端の予約を削除する。
        /// 経路を探索する際に呼ばないと以前の経路の末端の予約が残ったままになってしまう。
        /// </summary>
        void DeletePath()
        {
            if (_context.Path.Count > 0)
            {
                DeleteOnCell(GoalPos);
                _context.Path.Clear();
            }
        }

        Vector2Int World2Grid(in Vector3 pos)
        {
            return FieldManager.Instance.WorldPosToGridIndex(pos);
        }

        bool TryGetPath(Vector2Int from, Vector2Int to)
        {
            return FieldManager.Instance.TryGetPath(from, to, ref _context.Path);
        }

        void SetOnCell(in Vector3 pos)
        {
            FieldManager.Instance.SetActorOnCell(pos, _context.Type);
        }

        void DeleteOnCell(in Vector3 pos)
        {
            FieldManager.Instance.SetActorOnCell(pos, ActorType.None);
        }

        bool IsOnCell(in Vector3 pos)
        {
            return FieldManager.Instance.IsActorOnCell(pos, out ActorType _);
        }

        bool TryGetResourceCells(ResourceType type, out List<Cell> list)
        {
            return FieldManager.Instance.TryGetResourceCells(type, out list);
        }

        bool CreatePathIfNeighbourOnGrid(Vector2Int from, Vector2Int to)
        {
            return ActorHelper.CreatePathIfNeighbourOnGrid(from, to, _context);
        }
    }
}
