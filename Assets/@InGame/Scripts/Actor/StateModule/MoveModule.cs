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

        public MoveModule(DataContext context)
        {
            _context = context;
        }

        public bool OnNextCell => _context.Transform.position == NextCellPos;

        List<Vector3> Path => _context.Path;
        float MoveSpeed => _context.Base.MoveSpeed;
        Vector3 Position
        {
            get => _context.Transform.position;
            set => _context.Transform.position = value;
        }
        Quaternion Rotation
        {
            get => _context.Model.rotation;
            set => _context.Model.rotation = value;
        }

        /// <summary>
        /// 各値を既定値に戻す
        /// </summary>
        public void Reset()
        {
            CurrentCellPos = Position;
            NextCellPos = default;
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
            Reset();

            if (Path.Count > 0)
            {
                // 経路の先頭(次のセル)から1つ取り出す
                NextCellPos = Path[0];
                Path.RemoveAt(0);
                // 経路のセルとキャラクターの高さが違うので水平に移動させるために高さを合わせる
                NextCellPos.y = Position.y;

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