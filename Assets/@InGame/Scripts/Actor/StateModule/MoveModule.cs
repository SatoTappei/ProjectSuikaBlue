using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// ステートのセル間を移動する処理及び必要なメソッドを抜き出したクラス
    /// </summary>
    public class MoveModule
    {
        public Vector3 CurrentCellPos;
        public Vector3 NextCellPos;

        DataContext _context;
        float _lerpProgress;
        float _speedModify = 1;

        public bool OnNextCell => _context.Transform.position == NextCellPos;

        public MoveModule(DataContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 各値を既定値に戻す
        /// </summary>
        public void Reset()
        {
            CurrentCellPos = _context.Transform.position;
            NextCellPos = default;
            _lerpProgress = 0;
            _speedModify = 1;
        }

        public void Move()
        {
            _lerpProgress += Time.deltaTime * _context.Base.MoveSpeed * _speedModify;
            _context.Transform.position = Vector3.Lerp(CurrentCellPos, NextCellPos, _lerpProgress);
        }

        public void Look()
        {
            Vector3 dir = NextCellPos - CurrentCellPos;
            _context.Model.rotation = Quaternion.LookRotation(dir, Vector3.up);
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