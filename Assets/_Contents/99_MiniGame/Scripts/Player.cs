using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace MiniGame
{
    public class Player : ActorBase
    {
        [Space(20)]
        [SerializeField] Transform _muzzle;
        [SerializeField] Transform _turret;
        [SerializeField] PlayerBullet _bullet;
        [SerializeField] GameObject _defeatedPlayer;
        [Header("弾速")]
        [SerializeField] float _bulletSpeed = 25.0f;
        [Header("発射速度"), Min(0.1f)]
        [SerializeField] float _fireRate = 0.33f;

        AudioModule _audio;
        PlayerBulletPool _bulletPool;
        float _fireTimer;
        bool _isValid;

        /// <summary>
        /// 死亡したかどうかのフラグであり、操作可能/不可能のフラグとは別
        /// </summary>
        public bool IsDefeated { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            TryGetComponent(out _audio);
            CreateBulletPool();

            // ゲーム開始で操作可能/ゲームオーバーで操作不可能
            MessageBroker.Default.Receive<InGameStartMessage>().Subscribe(_ => _isValid = true).AddTo(this);
            MessageBroker.Default.Receive<GameOverMessage>().Subscribe(_ => _isValid = false).AddTo(this);
        }

        void Update()
        {
            if (!_isValid) return;

            LooaAtMouse();

            if      (Input.GetMouseButtonDown(0)) ExceedFireTimer();
            else if (Input.GetMouseButton(0) && StepFireTimer()) Fire();
            else if (Input.GetMouseButtonUp(0)) ResetFireTimer();
        }

        void CreateBulletPool()=> _bulletPool = new(_bullet, "PlayerBulletPool");

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

            if (_audio != null) _audio.Play(AudioKey.SeFire);
        }

        protected override void Defeated()
        {
            Invalid();
            PlayDefeatedEffect();

            IsDefeated = true;
            MessageBroker.Default.Publish(new PlayerDefeatedMessage());
        }

        /// <summary>
        /// スケールを0に変更＆コライダーの無効化で画面から消す
        /// </summary>
        protected override void Invalid()
        {
            transform.localScale = Vector3.zero;
            GetComponent<Collider>().enabled = false;
        }

        protected override void PlayDefeatedEffect()
        {
            if (_audio != null) _audio.Play(AudioKey.SeExplode);
            Instantiate(_defeatedPlayer, transform.position, Quaternion.identity);
        }
    }
}
