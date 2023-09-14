using UnityEngine;
using System;

namespace PSB.InGame
{
    public static class Detect
    {
        /// <summary>
        /// Contextの値でPhysics.OverlapSphereNonAllocを呼び出す。
        /// detectedの中身をクリアしてから処理を行う。
        /// </summary>
        /// <returns>検知した数</returns>
        public static int OverlapSphere(DataContext context, Collider[] detected)
        {
            Array.Clear(detected, 0, detected.Length);

            Vector3 pos = context.Transform.position;
            float radius = context.Base.SightRadius;
            LayerMask layer = context.Base.SightTargetLayer;
            return Physics.OverlapSphereNonAlloc(pos, radius, detected, layer);
        }
    }
}