using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    public class KurokamiSpawnController : MonoBehaviour
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

        /// <summary>
        /// ���Ԋu�ō����𐶐����鏈��
        /// ���ƂȂ�L�����N�^�[�̃��X�g��p���Đ����ӏ������߂�
        /// </summary>
        public void Tick(IReadOnlyList<Actor> candidate)
        {
            // �f�o�b�O���͎��Ԍo�߂Ő��������Ȃ�
            if (_isDebug) return;

            _timer += Time.deltaTime;
            if (_timer > _interval)
            {
                _timer = 0;
                TrySpawnKurokami(CalculateCenterPos(candidate));
            }
        }

        Vector3 CalculateCenterPos(IReadOnlyList<Actor> candidate)
        {
            Vector3 pos = Vector3.zero;
            if (candidate.Count == 0) return pos; // 0���Z��h��

            foreach (Actor actor in candidate)
            {
                pos += actor.transform.position;
            }
            return pos / candidate.Count;
        }

        /// <summary>
        /// �����̈ʒu�𒆐S�ɔ��ߖT�Ɉ��Ԋu���ꂽ�ʒu�ɐ�������
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

        /// <summary>
        /// �f�o�b�O�p: �����͈͂��M�Y���ɕ`�悷��
        /// </summary>
        public void DrawGizmos(in Vector3 pos)
        {
#if UNITY_EDITOR
            if (_isDebug) Gizmos.DrawWireSphere(pos, _spawnRadius);
#endif
        }
    }
}
