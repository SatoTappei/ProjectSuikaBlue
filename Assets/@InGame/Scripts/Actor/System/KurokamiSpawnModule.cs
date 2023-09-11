using System.Collections.Generic;
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

        public void StepSpawnFromCandidate(IReadOnlyList<Actor> candidate)
        {
            // デバッグ時は時間経過で生成させない
            if (_isDebug) return;

            _timer += Time.deltaTime;
            if (_timer > _interval)
            {
                _timer = 0;
                TrySpawnKurokami(CalculateCandidateCenterPos(candidate));
            }
        }

        /// <summary>
        /// 候補地の中心を求める
        /// </summary>
        /// <returns>群れの中心の座標</returns>
        Vector3 CalculateCandidateCenterPos(IReadOnlyList<Actor> candidate)
        {
            Vector3 pos = Vector3.zero;
            if (candidate.Count == 0) return pos; // 0除算を防ぐ

            foreach (Actor actor in candidate)
            {
                pos += actor.transform.position;
            }
            return pos / candidate.Count;
        }

        /// <summary>
        /// 引数の位置を中心に一定間隔離れた位置に生成する
        /// </summary>
        /// <returns>生成した:true 生成できなかった:false</returns>
        bool TrySpawnKurokami(in Vector3 pos)
        {
            foreach (Vector2Int dir in Utility.EightDirections.OrderBy(_ => System.Guid.NewGuid()))
            {
                Vector3 spawnPos = pos + new Vector3(dir.x, 0, dir.y) * _spawnRadius;
                if (!FieldManager.Instance.IsWithInGrid(spawnPos)) continue;
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
