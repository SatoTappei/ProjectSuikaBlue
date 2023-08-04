using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using MiniGameECS;

namespace MiniGame
{
    public class Enemy : ActorBase
    {
        [Space(20)]
        [SerializeField] Transform _model;
        [SerializeField] int _score = 100;
        [Header("�ړ����x�Ɋւ���l")]
        [Tooltip(" �ړ����������S�ɕς��܂ł̎��Ԃ� �ړ����x �� �Z���̑傫�� �ɉ����Ē��߂���")]
        [SerializeField] float _dirChangeDuration = 3.0f;
        [SerializeField] float _moveSpeed = 1.5f;

        AudioModule _audio;
        VectorFieldManager _vectorFieldManager;
        Vector3 _currentDir;
        /// <summary>
        /// �Ō�Ɏ��g�Ƀ_���[�W��^���������ێ����Ă���
        /// </summary>
        GameObject _lastAttacker;

        /// <summary>
        /// ���������ۂɁA���������K���ĂԕK�v������B�R���X�g���N�^�̑���
        /// </summary>
        public void Init(VectorFieldManager vectorFieldManager)
        {
            _vectorFieldManager = vectorFieldManager;
        }

        protected override void Awake()
        {
            base.Awake();
            TryGetComponent(out _audio);
        }

        protected override void Start()
        {
            base.Start();
            // ���ڃV�[���ɔz�u�����ꍇ�͒T���Ă���
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

        void LookAtDirection()
        {
            _model.forward = _currentDir;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagUtility.PlayerTag))
            {
                if (other.TryGetComponent(out IDamageable damageable)) damageable.Damage(gameObject);
                // �v���C���[�ƏՓ˂����ꍇ�͎��g�����j�����
                Defeated();
            }
        }

        protected override void Damaged(GameObject attacker, int _) => _lastAttacker = attacker;

        protected override void Defeated()
        {
            Invalid();
            PlayDefeatedEffect();
            MessageBroker.Default.Publish(new AddScoreMessage() { Score = _score });
        }

        /// <summary>
        /// �X�P�[����0�ɕύX���R���C�_�[�̖������ŉ�ʂ�������A1�b��ɍ폜����
        /// </summary>
        protected override void Invalid()
        {
            transform.localScale = Vector3.zero;
            GetComponent<Collider>().enabled = false;
            Destroy(gameObject, 1.0f);
        }

        protected override void PlayDefeatedEffect()
        {
            // ��
            if (_audio != null) _audio.Play(AudioKey.SeBlood);
            
            Vector3 effectDir;
            // �v���C���[�̒e�ɓ|���ꂽ�ꍇ�́A�e�̌����Ă������
            if (_lastAttacker != null && _lastAttacker.TryGetComponent(out PlayerBullet bullet))
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
