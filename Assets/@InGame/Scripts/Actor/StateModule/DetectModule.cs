using System;
using UnityEngine;

namespace PSB.InGame
{
    public class DetectModule
    {
        DataContext _context;

        public DetectModule(DataContext context)
        {
            _context = context;
        }

        Collider[] Detected => _context.Detected;

        /// <summary>
        /// Contextの値と半径を指定してPhysics.OverlapSphereNonAllocを呼び出す。
        /// detectedの中身をクリアしてから処理を行う。
        /// </summary>
        /// <returns>検知した数</returns>
        public int OverlapSphere(float radius)
        {
            Array.Clear(Detected, 0, Detected.Length);

            Vector3 pos = _context.Transform.position;
            LayerMask layer = _context.Base.SightTargetLayer;
            return Physics.OverlapSphereNonAlloc(pos, radius, Detected, layer);
        }
    }
}
