using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace MiniGame
{
    public class Player : ActorBase
    {
        [SerializeField] Transform _muzzle;
        [SerializeField] Transform _turret;
        [SerializeField] PlayerBullet _bullet;
        [Header("�e��")]
        [SerializeField] float _bulletSpeed = 25.0f;
        [Header("���ˑ��x")]
        [SerializeField] float _fireRate = 0.33f;

        PlayerBulletPool _bulletPool;
        float _fireTimer;
        bool _isValid;

        protected override void Awake()
        {
            base.Awake();
            CreateBulletPool();

            // �Q�[���J�n�ő���\/�Q�[���I�[�o�[�ő���s�\
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
        /// �C�����}�E�X�̕����Ɍ�����
        /// </summary>
        void LooaAtMouse()
        {
            // �}�E�X�ʒu
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.transform.position.y;
            mousePos = Camera.main.ScreenToWorldPoint(mousePos);
            // ������
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
            MessageBroker.Default.Publish(new PlayerDefeatedMessage());
        }
    }
}
