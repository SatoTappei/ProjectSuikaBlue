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
        /// Context�̒l�Ɣ��a���w�肵��Physics.OverlapSphereNonAlloc���Ăяo���B
        /// detected�̒��g���N���A���Ă��珈�����s���B
        /// </summary>
        /// <returns>���m������</returns>
        public int OverlapSphere(float radius)
        {
            Array.Clear(Detected, 0, Detected.Length);

            Vector3 pos = _context.Transform.position;
            LayerMask layer = _context.Base.SightTargetLayer;
            return Physics.OverlapSphereNonAlloc(pos, radius, Detected, layer);
        }
    }
}
