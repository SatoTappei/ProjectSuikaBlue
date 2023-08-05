using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MiniGame
{
    [RequireComponent(typeof(Damageable))]
    public class Player : MonoBehaviour
    {
        [SerializeField] Transform _muzzle;
        [SerializeField] Transform _turret;
        [SerializeField] PlayerBullet _bullet;
        [SerializeField] GameObject _defeatedPrefab;
        [SerializeField] Damageable _damageable;
        [Header("�e��")]
        [SerializeField] float _bulletSpeed = 25.0f;
        [Header("���ˑ��x"), Min(0.1f)]
        [SerializeField] float _fireRate = 0.33f;

        AudioModule _audio;
        PlayerBulletPool _bulletPool;
        float _fireTimer;
        bool _isValid;

        /// <summary>
        /// ���S�������ǂ����̃t���O�ł���A����\/�s�\�̃t���O�Ƃ͕�
        /// </summary>
        public bool IsDefeated { get; private set; }

        void Awake()
        {
            TryGetComponent(out _audio);
            CreateBulletPool();

            // �Q�[���J�n�ő���\/�Q�[���I�[�o�[�ő���s�\
            MessageBroker.Default.Receive<InGameStartMessage>()
                .Subscribe(_ => { _isValid = true; ResetStatus(); }).AddTo(this);
            MessageBroker.Default.Receive<GameOverMessage>()
                .Subscribe(_ => _isValid = false).AddTo(this);

            // ���j���ꂽ�ۂ̃R�[���o�b�N�ɓo�^/�폜
            _damageable.OnDefeated += Defeated;
            this.OnDestroyAsObservable().Subscribe(_ => _damageable.OnDefeated -= Defeated);
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

        void ResetStatus()
        {
            // �ő�̗͂Ƀ��Z�b�g
            IsDefeated = false;
            _damageable.ResetParams();
            // �L����
            Valid();
        }

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

            if (_audio != null) _audio.Play(AudioKey.SeFire);
        }

        void Defeated(GameObject _)
        {
            Invalid();
            PlayDefeatedEffect();

            // ���j���ꂽ�t���O�𗧂Ăă��b�Z�[�W���O
            IsDefeated = true;
            MessageBroker.Default.Publish(new PlayerDefeatedMessage());
        }

        void Valid()
        {
            transform.localScale = Vector3.one;
            GetComponent<Collider>().enabled = true;
        }

        void Invalid()
        {
            // �X�P�[����0�ɕύX���R���C�_�[�̖������ŉ�ʂ������
            transform.localScale = Vector3.zero;
            GetComponent<Collider>().enabled = false;
        }

        void PlayDefeatedEffect()
        {
            if (_audio != null) _audio.Play(AudioKey.SeExplode);
            Instantiate(_defeatedPrefab, transform.position, Quaternion.identity);
        }
    }
}
