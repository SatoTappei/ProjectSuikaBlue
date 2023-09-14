using UnityEngine;
using System;

namespace PSB.InGame
{
    public static class Detect
    {
        /// <summary>
        /// Context�̒l��Physics.OverlapSphereNonAlloc���Ăяo���B
        /// detected�̒��g���N���A���Ă��珈�����s���B
        /// </summary>
        /// <returns>���m������</returns>
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