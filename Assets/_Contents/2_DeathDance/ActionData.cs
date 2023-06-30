using UnityEngine;

namespace AnimationAction
{
    public class ActionData
    {
        string _animName;
        float _animLength;
        /// <summary>
        /// アニメーション再生までのディレイ
        /// </summary>
        float _beforeDelay;
        /// <summary>
        /// アニメーション再生後のディレイ
        /// </summary>
        float _afterDelay;
        /// <summary>
        /// アニメーション再生から攻撃判定が出るまでのディレイ
        /// </summary>
        float _attackDelay;
        /// <summary>
        /// 攻撃判定の持続時間
        /// </summary>
        float _attackDuration;
        /// <summary>
        /// アニメーション再生中に移動する方向
        /// </summary>
        Vector3 _moveDir;
        /// <summary>
        /// アニメーション再生中に移動する距離
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