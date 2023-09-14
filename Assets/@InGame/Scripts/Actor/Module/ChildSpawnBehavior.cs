using System.Collections;
using UniRx;
using UnityEngine;
using UnityEngine.Events;

namespace PSB.InGame
{
    public class ChildSpawnBehavior : MonoBehaviour
    {
        [SerializeField] DataContext _context;
        SpawnChildMessage _spawnChildMessage = new();
        Coroutine _spawnChild; // ������L�����Z���\�ɂ���
        bool _isMating;

        /// <summary>
        /// ������łȂ��ꍇ�A��莞�Ԍo�ߌ�A�q�����Y�ޏ������Ăяo���B
        /// �L�����Z�����邱�Ƃ��o����B
        /// </summary>
        public void SpawnChild(uint maleGene, UnityAction callback = null)
        {
            if (!_isMating)
            {
                _isMating = true;
                _spawnChild = StartCoroutine(SpawnChildCoroutine(maleGene, callback));
            }
        }

        /// <summary>
        /// ������̏ꍇ�A���̌�����L�����Z������
        /// �q�����Y�ރR���[�`�����~���A������t���O��܂�
        /// </summary>
        public void Cancel()
        {
            if (_spawnChild != null) StopCoroutine(_spawnChild);
            _isMating = false;
        }

        IEnumerator SpawnChildCoroutine(uint maleGene, UnityAction callback = null)
        {
            // ���o�Ƃ��ăp�[�e�B�N�������񂩏o��
            int c = 3; // �J��Ԃ��񐔂͓K��
            for (int i = 0; i < c; i++)
            {
                MessageBroker.Default.Publish(new PlayParticleMessage()
                {
                    Type = ParticleType.Mating,
                    Pos = transform.position,
                });
                yield return new WaitForSeconds(_context.Base.MatingTime / c);
            }
            // ���͔��ߖT�̃Z���Ɏq�����Y��
            if (TryGetNeighbourPos(out Vector3 pos))
            {
                _spawnChildMessage.Gene1 = maleGene;
                _spawnChildMessage.Gene2 = _context.Gene;
                _spawnChildMessage.Food = _context.Food.Value;
                _spawnChildMessage.Water = _context.Water.Value;
                _spawnChildMessage.HP = _context.HP.Value;
                _spawnChildMessage.LifeSpan = _context.LifeSpan.Value;
                _spawnChildMessage.Pos = pos;
                MessageBroker.Default.Publish(_spawnChildMessage);

                // �q�����Y�񂾂̂ŔɐB����0�ɂ���
                _context.BreedingRate.Value = 0;
            }

            yield return null;
            _isMating = false;
            callback?.Invoke();
        }

        /// <summary>
        /// ���͔��ߖT�̃Z���𒲂ׁA�q�𐶐�����ʒu���擾����
        /// </summary>
        /// <returns>�擾�o����:true ��������Z��������:false</returns>
        bool TryGetNeighbourPos(out Vector3 pos)
        {
            Vector2Int index = FieldManager.Instance.WorldPosToGridIndex(transform.position);
            foreach (Vector2Int dir in Utility.EightDirections)
            {
                Vector2Int neighbourIndex = index + dir;
                // ���n�������������A�L�����N�^�[�����Ȃ��Z��
                if (!FieldManager.Instance.TryGetCell(neighbourIndex, out Cell cell)) continue;
                if (!cell.IsEmpty) continue;

                // �������鍂�������g�̍����ɍ��킹��
                pos = cell.Pos;
                pos.y = transform.position.y;

                return true;
            }

            pos = Vector3.zero;
            return false;
        }
    }
}
