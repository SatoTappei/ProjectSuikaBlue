using UnityEngine;

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

        /// <summary>
        /// �^�O�Ŏ擾
        /// </summary>
        /// <returns>�����A�����A�������[�_�[�̏ꍇ:Actor ����ȊO:null</returns>
        public DataContext SearchTarget(string tag)
        {
            Transform transform = _context.Transform;
            float radius = _context.Base.SightRadius;
            LayerMask layer = _context.Base.SightTargetLayer;

            int count = Physics.OverlapSphereNonAlloc(transform.position, radius, _results, layer);
            if (count == 0) return null;

            foreach (Collider collider in _results)
            {
                if (collider == null) break;
                if (collider.transform == transform) continue; // ������e��
                if (!collider.CompareTag(tag)) continue;

                if (collider.TryGetComponent(out DataContext actor)) return actor;
            }

            return null;
        }
    }
}