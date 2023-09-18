using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// ステートのFieldManagerへセルの予約/予約解除を行う処理のラッパー
    /// </summary>
    public class FieldModule
    {
        readonly DataContext _context;

        public FieldModule(DataContext context)
        {
            _context = context;
        }

        public void SetOnCell(in Vector3 pos)
        {
            FieldManager.Instance.SetActorOnCell(pos, _context.Type);
        }

        public void SetOnCell(Vector2Int index)
        {
            FieldManager.Instance.SetActorOnCell(index, _context.Type);
        }

        public void DeleteOnCell(in Vector3 pos)
        {
            FieldManager.Instance.SetActorOnCell(pos, ActorType.None);
        }

        public void DeleteOnCell(Vector2Int index)
        {
            FieldManager.Instance.SetActorOnCell(index, ActorType.None);
        }

        /// <summary>
        /// 経路がある場合は経路の末端のセルの予約を削除する
        /// </summary>
        public void DeletePathGoalOnCell()
        {
            Vector3 pos = _context.Path[_context.Path.Count - 1];
            FieldManager.Instance.SetActorOnCell(pos, ActorType.None);
        }
    }
}
