using UnityEngine;

namespace AnimationAction
{
    public class ActionData
    {
        string _animName;
        float _animLength;
        /// <summary>
        /// �A�j���[�V�����Đ��܂ł̃f�B���C
        /// </summary>
        float _beforeDelay;
        /// <summary>
        /// �A�j���[�V�����Đ���̃f�B���C
        /// </summary>
        float _afterDelay;
        /// <summary>
        /// �A�j���[�V�����Đ�����U�����肪�o��܂ł̃f�B���C
        /// </summary>
        float _attackDelay;
        /// <summary>
        /// �U������̎�������
        /// </summary>
        float _attackDuration;
        /// <summary>
        /// �A�j���[�V�����Đ����Ɉړ��������
        /// </summary>
        Vector3 _moveDir;
        /// <summary>
        /// �A�j���[�V�����Đ����Ɉړ����鋗��
        /// </summary>
        float _moveDistance;

        public ActionData(string animName)
        {
            _animName = animName;
        }

        public ActionData(string animName, float animLength, float beforeDelay, float afterDelay,
            float attackDelay, float attackDuration, Vector3 moveDir, float moveDistance)
        {
            _animName = animName;
            _animLength = animLength;
            _beforeDelay = beforeDelay;
            _afterDelay = afterDelay;
            _attackDelay = attackDelay;
            _attackDuration = attackDuration;
            _moveDir = moveDir;
            _moveDistance = moveDistance;
        }

        public string AnimName => _animName;
        public float AnimLength => _animLength;
        public float BeforeDelay => _beforeDelay;
        public float AfterDelay => _afterDelay;
        public float AttackDelay => _attackDelay;
        public float AttackDuration => _attackDuration;
        public Vector3 MoveDir => _moveDir;
        public float MoveDistance => _moveDistance;
    }
}