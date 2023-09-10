using System.Linq;
using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    public class KurokamiSpawnModule : MonoBehaviour
    {
        [SerializeField] float _spawnRadius = 10.0f;
        [SerializeField] float _interval = 0.5f;
        [Header("�f�o�b�O�p:�L�[���͂œG�𐶐�����")]
        [SerializeField] bool _isDebug;

        float _timer;

#if UNITY_EDITOR
        void Update()
        {
            if (_isDebug && Input.GetKeyDown(KeyCode.Space))
            {
                TrySpawnKurokami(Vector3.zero);
            }
        }
#endif

        public void Step(in Vector3 spawnPos)
        {
            // �f�o�b�O���͎��Ԍo�߂Ő��������Ȃ�
            if (_isDebug) return;

            _timer += Time.deltaTime;
            if (_timer > _interval)
            {
                _timer = 0;
                TrySpawnKurokami(spawnPos);
            }
        }

        /// <summary>
        /// ���[�_�[�̈ʒu�𒆐S�Ɉ��Ԋu���ꂽ�ʒu�ɐ�������
        /// </summary>
        /// <returns>��������:true �����ł��Ȃ�����:false</returns>
        bool TrySpawnKurokami(in Vector3 pos)
        {
            foreach (Vector2Int dir in Utility.EightDirections.OrderBy(_ => System.Guid.NewGuid()))
            {
                Vector3 spawnPos = pos + new Vector3(dir.x, 0, dir.y) * _spawnRadius;

                // �Z�����擾�o�����B�Z�����C�ȊO�A�����Ȃ��A�L���������Ȃ��ꍇ�͐����\
                if (!FieldManager.Instance.TryGetCell(spawnPos, out Cell cell)) continue;
                if (!cell.IsEmpty) continue;

                MessageBroker.Default.Publish(new KurokamiSpawnMessage() { Pos = cell.Pos });
                return true;
            }

            return false;
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (_isDebug)
            {
                // �f�o�b�O�p��000�̈ʒu�𒆐S�ɐ����͈͂�`��
                Gizmos.DrawWireSphere(Vector3.zero, _spawnRadius);
            }
#endif
        }
    }
}
