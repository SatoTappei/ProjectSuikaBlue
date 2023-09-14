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

        /// <summary>
        /// 一定間隔で黒髪を生成する処理
        /// 候補となるキャラクターのリストを用いて生成箇所を求める
        /// </summary>
        public void Tick(IReadOnlyList<Actor> candidate)
        {
            // デバッグ時は時間経過で生成させない
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
            if (candidate.Count == 0) return pos; // 0除算を防ぐ

            foreach (Actor actor in candidate)
            {
                pos += actor.transform.position;
            }
            return pos / candidate.Count;
        }

        /// <summary>
        /// 引数の位置を中心に八近傍に一定間隔離れた位置に生成する
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

        /// <summary>
        /// デバッグ用: 生成範囲をギズモに描画する
        /// </summary>
        public void DrawGizmos(in Vector3 pos)
        {
#if UNITY_EDITOR
            if (_isDebug) Gizmos.DrawWireSphere(pos, _spawnRadius);
#endif
        }
    }
}
