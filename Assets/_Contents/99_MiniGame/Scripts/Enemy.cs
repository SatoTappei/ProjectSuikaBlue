using MiniGameECS;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MiniGame
{
    [RequireComponent(typeof(Damageable))]
    public class Enemy : MonoBehaviour
    {
        [SerializeField] Transform _model;
        [SerializeField] Damageable _damageable;
        [Header("���j���X�R�A")]
        [SerializeField] int _score = 100;
        [Header("�ړ����x�Ɋւ���l")]
        [Tooltip(" �ړ����������S�ɕς��܂ł̎��Ԃ� �ړ����x �� �Z���̑傫�� �ɉ����Ē��߂���")]
        [SerializeField] float _dirChangeDuration = 3.0f;
        [SerializeField] float _moveSpeed = 1.5f;

        AudioModule _audio;
        VectorFieldManager _vectorFieldManager;
        Vector3 _currentDir;

        /// <summary>
        /// ���������ۂɁA���������K���ĂԕK�v������B�R���X�g���N�^�̑���
        /// </summary>
        public void Init(VectorFieldManager vectorFieldManager)
        {
            _vectorFieldManager = vectorFieldManager;
        }

        void Awake()
        {
            TryGetComponent(out _audio);

            // ���߂��ׂ�ɂȂ����������A�X�R�A�͓���Ȃ��B
            MessageBroker.Default.Receive<GameOverMessage>()
                .Subscribe(_ => { Invalid(); PlayDefeatedEffect(gameObject); }).AddTo(this);

            // ���j���ꂽ�ۂ̃R�[���o�b�N�ɓo�^/�폜
            _damageable.OnDefeated += Defeated;
            this.OnDestroyAsObservable().Subscribe(_ => _damageable.OnDefeated -= Defeated);
        }

        void Start()
        {
            // �{���̓X�|�i�[���琶������邪�A���ڃV�[���ɔz�u�����ꍇ�͒T���Ă���
            _vectorFieldManager ??= FindFirstObjectByType<VectorFieldManager>();
        }

        void Update()
        {
            FollowVector();
            LookAtDirection();
        }

        /// <summary>
        /// �x�N�g���t�B�[���h��̌��ݎ��g������Z���̃x�N�g���ɉ����Ĉړ�����
        /// �ȉ��̏����𖞂����Ă���K�v������B
        /// 1.�x�N�g���t�B�[���h�͏㉺���E��4����
        /// 2.DirChangeDuration��MoveSpeed�𒲐����ė����Ƃ��Z���̒��S��ʂ�悤�Ȓl�ɂȂ��Ă���B
        /// </summary>
        void FollowVector()
        {
            Vector3 vector = _vectorFieldManager.GetCellVector(transform.position);
            // ���`�⊮���邱�ƂŁA�Z�����ׂ������Ƀx�N�g�����ς���������ŁA�Z���̕ӏ���ړ�����̂�h���B
            _currentDir = Vector3.Lerp(_currentDir, vector, Time.deltaTime * _dirChangeDuration);
            transform.Translate(_currentDir.normalized * Time.deltaTime * _moveSpeed);
        }

        void LookAtDirection() => _model.forward = _currentDir;

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagUtility.PlayerTag))
            {
                // �v���C���[�ƏՓ˂����ꍇ�͎��g�����j�����
                other.GetComponent<IDamageable>().Damage(1, gameObject);
                Defeated(gameObject);
            }
        }

        void Defeated(GameObject attacker)
        {
            Invalid();
            PlayDefeatedEffect(attacker);
            MessageBroker.Default.Publish(new AddScoreMessage() { Score = _score });
        }

        void Invalid()
        {
            // �X�P�[����0�ɕύX���R���C�_�[�̖������ŉ�ʂ�������A1�b��ɍ폜����
            transform.localScale = Vector3.zero;
            GetComponent<Collider>().enabled = false;
            Destroy(gameObject, 1.0f);
        }

        void PlayDefeatedEffect(GameObject attacker)
        {
            // ��
            if (_audio != null) _audio.Play(AudioKey.SeBlood);
            
            Vector3 effectDir;
            // �v���C���[�̒e�ɓ|���ꂽ�ꍇ�́A�e�̌����Ă������
            if (attacker.TryGetComponent(out PlayerBullet bullet))
            {
                effectDir = bullet.Forward;
            }
            // �v���C���[�ւ̌��˂ȂǂŎ��S�����ꍇ�͎��g�̌������փG�t�F�N�g���o��
            else
            {
                effectDir = -_model.forward;
            }
            MonoToEcsTransfer.Instance.AddData(transform.position, effectDir, EntityType.Debris);
        }
    }
}
