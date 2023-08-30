using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    public class SenilityState : BaseState
    {
        public SenilityState(DataContext context) : base(context, StateType.Senility)
        {
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
            Context.Transform.GetComponent<SphereCollider>().enabled = false;
            Context.Transform.GetComponentInChildren<SkinnedMeshRenderer>().enabled = false;
        }

        void SendMessage()
        {
            // 死んだメッセージ
            MessageBroker.Default.Publish(new ActorDeathMessage()
            {
                Pos = Context.Transform.position,
                Type = ActionType.Senility,
            });
            // ログ
            string r = Utility.ColorCodeRed;
            string g = Utility.ColorCodeGreen;
            MessageBroker.Default.Publish(new EventLogMessage()
            {
                Message = $"<color={g}>{Context.Transform.name}</color>が<color={r}>大往生した</color>。ご臨終、だな。",
            });
        }
    }
}
