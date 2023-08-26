using UnityEngine;

namespace PSB.InGame
{
    public class SightSensor : MonoBehaviour
    {
        [SerializeField] float _radius;
        [SerializeField] LayerMask _targetLayer;

        Collider[] _results = new Collider[8];

        public Actor SearchEnemy()
        {
            int count = Physics.OverlapSphereNonAlloc(transform.position, _radius, _results, _targetLayer);
            if (count > 0)
            {
                foreach (Collider collider in _results)
                {
                    if (collider == null) break;
                    if (collider.gameObject == gameObject) continue; // ������e��
                    if (!collider.TryGetComponent(out Kurokami k)) continue;

                    // TODO:���t���[���Ă΂��悤�ȃ��\�b�h�ŃR���|�[�l���g�̎擾�����Ă���
                    if (collider.TryGetComponent(out Actor actor)) return actor;
                }

                return null;
            }
            else
            {
                return null;
            }
        }

        void OnDrawGizmos()
        {
            //Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }
}