using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public class Enemy : MonoBehaviour
    {
        [SerializeField] LayerMask _mask;
        VectorFieldManager _vectorFieldManager;

        /// <summary>
        /// 生成した際に、生成側が必ず呼ぶ必要がある。コンストラクタの代わり
        /// </summary>
        public void Init(VectorFieldManager vectorFieldManager)
        {
            _vectorFieldManager = vectorFieldManager;
        }

        void Start()
        {
            // 直接シーンに配置した場合は探してくる
            _vectorFieldManager ??= FindFirstObjectByType<VectorFieldManager>();
        }

        void Update()
        {
            Vector3 vector = _vectorFieldManager.GetCellVector(transform.position);
            Debug.Log(vector);

            //Vector3 rayOrigin = transform.position + Vector3.up * 0.25f;
            //Physics.Raycast(rayOrigin, new Vector3(-1, 0, 0), out RaycastHit hit, 1.0f, _mask);
            //if (hit.collider != null)
            //{
            //    float dist = (rayOrigin - hit.point).sqrMagnitude;
            //    float percent = dist / 1.0f;
            //    Debug.Log("障害物までの距離: " + percent);

            //    //vector += (new Vector3(0, 0, 1.0f) * (2.0f -percent));
            //}
            
            //Debug.DrawRay(transform.position + Vector3.up * 0.25f, new Vector3(-1, 0, 0));
            //Vector3 rayHitPos = 

            //if (_currentDir != vector)
            //{
            //    _prevDir = _currentDir;
            //    _currentDir = vector;
            //}
            //_currentDir = Vector3.Lerp(_prevDir, _currentDir, Time.deltaTime * 3.0f);

            transform.Translate(vector.normalized * Time.deltaTime * 1.5f);
        }
    }
}

// ベクトルフィールド使い方どうしよう問題
// 次のセルに映った途端切り替わってしまうのでセルの返上を移動するみたいな挙動になる
// Lerpを使って補完する？これもセルの中央に居ること前提
