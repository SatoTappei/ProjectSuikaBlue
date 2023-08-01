using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace MiniGame
{
    public class Enemy : ActorBase
    {
        [SerializeField] int _score = 100;
        [Header("�ړ����x�Ɋւ���l")]
        [Tooltip(" �ړ����������S�ɕς��܂ł̎��Ԃ� �ړ����x �� �Z���̑傫�� �ɉ����Ē��߂���")]
        [SerializeField] float _dirChangeDuration = 3.0f;
        [SerializeField] float _moveSpeed = 1.5f;

        VectorFieldManager _vectorFieldManager;
        Vector3 _currentDir;

        /// <summary>
        /// ���������ۂɁA���������K���ĂԕK�v������B�R���X�g���N�^�̑���
        /// </summary>
        public void Init(VectorFieldManager vectorFieldManager)
        {
            _vectorFieldManager = vectorFieldManager;
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

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(TagUtility.PlayerTag))
            {
                if (other.TryGetComponent(out IDamageable damageable)) damageable.Damage();
                // �v���C���[�ƏՓ˂����ꍇ�͎��g�����j�����
                Defeated();
            }
        }

        protected override void Defeated()
        {
            MessageBroker.Default.Publish(new AddScoreMessage() { Score = _score });
            Destroy(gameObject);
        }
    }
}
