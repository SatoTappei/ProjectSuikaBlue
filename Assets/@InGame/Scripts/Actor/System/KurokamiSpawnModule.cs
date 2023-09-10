using System.Linq;
using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    public class KurokamiSpawnModule : MonoBehaviour
    {
        [SerializeField] float _spawnRadius = 10.0f;
        [SerializeField] float _interval = 0.5f;
        [Header("デバッグ用:キー入力で敵を生成する")]
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
            // デバッグ時は時間経過で生成させない
            if (_isDebug) return;

            _timer += Time.deltaTime;
            if (_timer > _interval)
            {
                _timer = 0;
                TrySpawnKurokami(spawnPos);
            }
        }

        /// <summary>
        /// リーダーの位置を中心に一定間隔離れた位置に生成する
        /// </summary>
        /// <returns>生成した:true 生成できなかった:false</returns>
        bool TrySpawnKurokami(in Vector3 pos)
        {
            foreach (Vector2Int dir in Utility.EightDirections.OrderBy(_ => System.Guid.NewGuid()))
            {
                Vector3 spawnPos = pos + new Vector3(dir.x, 0, dir.y) * _spawnRadius;

                // セルが取得出来た。セルが海以外、資源なし、キャラがいない場合は生成可能
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
                // デバッグ用に000の位置を中心に生成範囲を描画
                Gizmos.DrawWireSphere(Vector3.zero, _spawnRadius);
            }
#endif
        }
    }
}
