using UnityEngine;
using System;

namespace PSB.InGame
{
    public class SightSensor
    {
        readonly DataContext _context;
        Collider[] _results = new Collider[8];

        public SightSensor(DataContext context)
        {
            _context = context;
        }

        public bool TrySearchTarget(string tag, out DataContext target)
        {
            Array.Clear(_results, 0, _results.Length);

            Transform transform = _context.Transform;
            float radius = _context.Base.SightRadius;
            LayerMask layer = _context.Base.SightTargetLayer;

            int count = Physics.OverlapSphereNonAlloc(transform.position, radius, _results, layer);
            if (count == 0)
            {
                target = null;
                return false;
            }

            foreach (Collider collider in _results)
            {
                if (collider == null) break;
                if (collider.transform == transform) continue; // Ž©•ª‚ð’e‚­
                if (!collider.CompareTag(tag)) continue;

                if (collider.TryGetComponent(out target)) return true;
            }

            target = null;
            return false;
        }
    }
}