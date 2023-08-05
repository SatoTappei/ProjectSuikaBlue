using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlayerBullet : MonoBehaviour
    {
        /// <summary>
        /// �q�I�u�W�F�N�g��3D���f�����ԕ����ɉ�]�����邽�߂ɖ��O�Ŏ擾����
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
        /// �v���C���[�����e�𔭎˂����ۂɌĂԕK�v������
        /// �e����Ԃ��߂ɕK�v�Ȓl��ݒ肷��
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