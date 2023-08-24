using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    public class KilledState : BaseState
    {
        IBlackBoardForState _blackBoard;

        public KilledState(IBlackBoardForState blackBoard) : base(StateType.Killed)
        {
            _blackBoard = blackBoard;
        }

        protected override void Enter()
        {
            Invalid();
            SendMessage(); // <- このメッセージをコントローラー側が受信すると死亡を受信できる
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
        }

        void Invalid()
        {
            // コライダーとレンダラーを無効化して、クリックを防ぐ＆画面に非表示
            _blackBoard.Transform.GetComponent<SphereCollider>().enabled = false;
            _blackBoard.Transform.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        }

        void SendMessage()
        {
            // 死んだメッセージ
            MessageBroker.Default.Publish(new ActorDeathMessage()
            {
                Pos = _blackBoard.Transform.position,
                Type = ActionType.Killed,
            });
        }
    }
}
