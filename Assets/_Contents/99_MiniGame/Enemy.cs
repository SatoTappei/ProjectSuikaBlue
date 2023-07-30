using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public class Enemy : MonoBehaviour
    { 
        [Header("移動速度に関する値")]
        [Tooltip(" 移動方向が完全に変わるまでの時間は 移動速度 と セルの大きさ に応じて調節する")]
        [SerializeField] float _dirChangeDuration = 3.0f;
        [SerializeField] float _moveSpeed = 1.5f;

        VectorFieldManager _vectorFieldManager;
        Vector3 _currentDir;

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
            FollowVector();
        }

        /// <summary>
        /// ベクトルフィールド上の現在自身がいるセルのベクトルに沿って移動する
        /// 以下の条件を満たしている必要がある。
        /// 1.ベクトルフィールドは上下左右の4方向
        /// 2.DirChangeDurationとMoveSpeedを調整して両方ともセルの中心を通るような値になっている。
        /// </summary>
        void FollowVector()
        {
            Vector3 vector = _vectorFieldManager.GetCellVector(transform.position);
            // 線形補完することで、セルを跨いだ時にベクトルが変わったせいで、セルの辺上を移動するのを防ぐ。
            _currentDir = Vector3.Lerp(_currentDir, vector, Time.deltaTime * _dirChangeDuration);
            transform.Translate(_currentDir.normalized * Time.deltaTime * _moveSpeed);
        }
    }
}
