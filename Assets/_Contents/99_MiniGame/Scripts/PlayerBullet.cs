using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerBullet : MonoBehaviour
    {
        /// <summary>
        /// 子オブジェクトの3Dモデルを飛ぶ方向に回転させるために名前で取得する
        /// </summary>
        const string ModelName= "Model";

        PlayerBulletPool _pool;
        Rigidbody _rigidbody;
        Transform _model;
        Vector3 _dir;
        float _speed;

        public Vector3 Forward => _model.forward;

        public void Init(PlayerBulletPool pool) => _pool = pool;

        void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _model = transform.Find(ModelName);
        }

        void FixedUpdate()
        {
            _rigidbody.velocity = _dir * _speed;
            _speed *= 0.98f;
        }

        /// <summary>
        /// プレイヤー側が弾を発射した際に呼ぶ必要がある
        /// 弾が飛ぶために必要な値を設定する
        /// </summary>
        public void SetBulletParamsOnFire(Transform muzzle, in Vector3 dir, float speed)
        {
            transform.position = muzzle.position;
            _model.forward = dir;
            _dir = dir;
            _speed = speed;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagUtility.EnemyTag)) OnEnemyHit(other);
            else if (other.CompareTag(TagUtility.ObstacleTag)) OnObstacleHit(other);
        }

        void OnEnemyHit(Collider other)
        {
            if (other.TryGetComponent(out IDamageable damageable)) damageable.Damage(1, gameObject);

            _pool.Return(this);
            ResetParams();
        }

        void OnObstacleHit(Collider other)
        {
            _pool.Return(this);
            ResetParams();
        }

        void ResetParams()
        {
            _rigidbody.velocity = Vector3.zero;
            _rigidbody.angularVelocity = Vector3.zero;
        }
    }
}