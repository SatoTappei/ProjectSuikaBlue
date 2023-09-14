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
        Coroutine _spawnChild; // 交尾をキャンセル可能にする
        bool _isMating;

        /// <summary>
        /// 交尾中でない場合、一定時間経過後、子供を産む処理を呼び出す。
        /// キャンセルすることが出来る。
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
        /// 交尾中の場合、その交尾をキャンセルする
        /// 子供を産むコルーチンを停止し、交尾中フラグを折る
        /// </summary>
        public void Cancel()
        {
            if (_spawnChild != null) StopCoroutine(_spawnChild);
            _isMating = false;
        }

        IEnumerator SpawnChildCoroutine(uint maleGene, UnityAction callback = null)
        {
            // 演出としてパーティクルを何回か出す
            int c = 3; // 繰り返す回数は適当
            for (int i = 0; i < c; i++)
            {
                MessageBroker.Default.Publish(new PlayParticleMessage()
                {
                    Type = ParticleType.Mating,
                    Pos = transform.position,
                });
                yield return new WaitForSeconds(_context.Base.MatingTime / c);
            }
            // 周囲八近傍のセルに子供を産む
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

                // 子供を産んだので繁殖率を0にする
                _context.BreedingRate.Value = 0;
            }

            yield return null;
            _isMating = false;
            callback?.Invoke();
        }

        /// <summary>
        /// 周囲八近傍のセルを調べ、子を生成する位置を取得する
        /// </summary>
        /// <returns>取得出来た:true 生成するセルが無い:false</returns>
        bool TryGetNeighbourPos(out Vector3 pos)
        {
            Vector2Int index = FieldManager.Instance.WorldPosToGridIndex(transform.position);
            foreach (Vector2Int dir in Utility.EightDirections)
            {
                Vector2Int neighbourIndex = index + dir;
                // 陸地かつ資源が無く、キャラクターがいないセル
                if (!FieldManager.Instance.TryGetCell(neighbourIndex, out Cell cell)) continue;
                if (!cell.IsEmpty) continue;

                // 生成する高さを自身の高さに合わせる
                pos = cell.Pos;
                pos.y = transform.position.y;

                return true;
            }

            pos = Vector3.zero;
            return false;
        }
    }
}
