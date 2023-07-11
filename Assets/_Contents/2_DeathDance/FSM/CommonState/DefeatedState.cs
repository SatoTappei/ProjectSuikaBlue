using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
    /// <summary>
    /// キャラクターが死亡した際の状態
    /// ボスと敵で共通の状態
    /// </summary>
    public class DefeatedState : EnemyStateBase
    {
        CommonLayerBlackBoard _blackBoard;

        public DefeatedState(EnemyStateType type, CommonLayerBlackBoard blackBoard) : base(type)
        {
            _blackBoard = blackBoard;
        }

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
        }
    }
}
