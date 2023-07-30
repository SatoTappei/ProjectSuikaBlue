using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public class Enemy : MonoBehaviour
    { 
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

        void Start()
        {
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
    }
}
