using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public class Player : ActorBase
    {
        [SerializeField] Transform _muzzle;
        [SerializeField] Transform _turret;
        [SerializeField] PlayerBullet _bullet;
        [Header("弾速")]
        [SerializeField] float _bulletSpeed = 25.0f;
        [Header("発射速度")]
        [SerializeField] float _fireRate = 0.33f;

        PlayerBulletPool _bulletPool;
        float _fireTimer;

        protected override void Awake()
        {
            base.Awake();
            _bulletPool = new(_bullet, "PlayerBulletPool");
        }

        void Update()
        {
            LooaAtMouse();

            if      (Input.GetMouseButtonDown(0)) ExceedFireTimer();
            else if (Input.GetMouseButton(0) && StepFireTimer()) Fire();
            else if (Input.GetMouseButtonUp(0)) ResetFireTimer();
        }

        /// <summary>
        /// 砲塔をマウスの方向に向ける
        /// </summary>
        void LooaAtMouse()
        {
            // マウス位置
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.transform.position.y;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            // 向ける
            Vector3 dir = mousePos - _turret.position;
            dir.y = _turret.position.y;
            Quaternion rot = Quaternion.LookRotation(dir);
            rot.x = 0;
            rot.z = 0;
            _turret.rotation = rot;
        }

        void ExceedFireTimer() => _fireTimer = _fireRate;
        void ResetFireTimer() => _fireTimer = 0;

        bool StepFireTimer()
        {
            _fireTimer += Time.deltaTime;
            if (_fireTimer > _fireRate)
            {
                _fireTimer = 0;
                return true;
            }
            return false;
        }

        void Fire()
        {
            PlayerBullet bullet = _bulletPool.Rent();
            bullet.SetBulletParamsOnFire(_muzzle, _turret.forward, _bulletSpeed);
        }

        protected override void Defeated()
        {
            gameObject.SetActive(false);
        }
    }
}
