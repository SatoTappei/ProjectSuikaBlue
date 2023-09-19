using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// ステートのセル間を移動する処理及び必要なメソッドを抜き出したクラス
    /// </summary>
    public class MoveModule
    {
        readonly DataContext _context;

        public Vector3 CurrentCellPos;
        public Vector3 NextCellPos;

        float _lerpProgress;
        float _speedModify = 1;
        int cellIndex = 0;

        public MoveModule(DataContext context)
        {
            _context = context;
        }

        public bool OnNextCell => Position == NextCellPos;

        List<Vector3> Path => _context.Path;
        float MoveSpeed => _context.Base.MoveSpeed;
        Vector3 Position
        {
            get => _context.Transform.position;
            set => _context.Transform.position = value;
        }
        Quaternion Rotation
        {
            set => _context.Model.rotation = value;
        }

        public void Reset()
        {
            cellIndex = 0;
            OnCell();
        }

        void OnCell()
        {
            CurrentCellPos = Position;
            NextCellPos = Position;
            _lerpProgress = 0;
            _speedModify = 1;
        }

        /// <summary>
        /// 各値を既定値に戻すことで、現在のセルの位置を自身の位置で更新する。
        /// 次のセルの位置をあれば次のセルの位置、なければ自身の位置で更新する。
        /// </summary>
        /// <returns>次のセルがある:true 次のセルが無い(目的地に到着):false</returns>
        public bool TryStepNextCell()
        {
            OnCell();

            if (cellIndex < Path.Count)
            {
                // 経路の先頭(次のセル)から1つ取り出す
                NextCellPos = Path[cellIndex++];
                // 経路のセルとキャラクターの高さが違うので水平に移動させるために高さを合わせる
                NextCellPos.y = Position.y;

                // TODO:次のセルとの距離が1セル以上あるかの判定を行っている
                Vector2Int currentIndex = FieldManager.Instance.WorldPosToGridIndex(CurrentCellPos);
                Vector2Int nextIndex = FieldManager.Instance.WorldPosToGridIndex(NextCellPos);
                if (!ActorHelper.IsNeighbourOnGrid(currentIndex, nextIndex))
                {
                    NextCellPos = Position;
                    return false;
                }
                #region デバッグ用: 次のセルの距離が2以上になってる？
                //var i1 = FieldManager.Instance.WorldPosToGridIndex(CurrentCellPos);
                //var i2 = FieldManager.Instance.WorldPosToGridIndex(NextCellPos);
                //if (!ActorHelper.IsNeighbourOnGrid(i1, i2))
                //{
                //    int dx = Mathf.Abs(i1.x - i2.x);
                //    int dy = Mathf.Abs(i1.y - i2.y);
                //    FieldManager.Instance.TryGetCell(i1, out Cell c1);
                //    FieldManager.Instance.TryGetCell(i2, out Cell c2);
                //    Debug.Log("経路が変 " + _context.Type + " " + _context.name + ": " + _context.GetComponent<Actor>().State + " 距離x:" + dx.ToString() + "距離y:" + dy.ToString());
                //    var v1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //    v1.transform.position = c1.Pos + Vector3.up * 0.5f;
                //    var v2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
                //    v2.transform.position = c2.Pos + Vector3.up * 0.5f;

                //}
                #endregion

                Modify();
                Look();
                return true;
            }
            else
            {
                NextCellPos = Position;
                return false;
            }
        }

        public void Move()
        {
            _lerpProgress += Time.deltaTime * MoveSpeed * _speedModify;
            Position = Vector3.Lerp(CurrentCellPos, NextCellPos, _lerpProgress);
        }

        public void Look()
        {
            Vector3 dir = NextCellPos - CurrentCellPos;

            if (dir != Vector3.zero)
            {
                Rotation = Quaternion.LookRotation(dir, Vector3.up);
            }
        }

        /// <summary>
        /// 斜め移動の速度を補正する
        /// </summary>
        public void Modify()
        {
            bool dx = Mathf.Approximately(CurrentCellPos.x, NextCellPos.x);
            bool dz = Mathf.Approximately(CurrentCellPos.z, NextCellPos.z);

            _speedModify = (dx || dz) ? 1 : 0.7f;
        }
    }
}