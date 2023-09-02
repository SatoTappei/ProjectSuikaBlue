using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// 死亡した際にコライダーとレンダラーを無効化。
    /// パーティクルの発生、ログに表示するメッセージを送信する。
    /// </summary>
    public class DeathState : BaseState
    {
        ParticleType _particleType;
        string _msg;

        public DeathState(DataContext context, StateType stateType, ParticleType particleType, string msg) 
            : base(context, stateType)
        {
            _particleType = particleType;
            _msg = msg;
        }

        protected override void Enter()
        {
            Invalid();
            SendParticleMessage(); 
            SendLogMessage();
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

        void SendParticleMessage()
        {
            MessageBroker.Default.Publish(new PlayParticleMessage()
            {
                Pos = Context.Transform.position,
                Type = _particleType,
            });
        }

        void SendLogMessage()
        {
            string r = Utility.ColorCodeRed;
            string g = Utility.ColorCodeGreen;
            string msg = $"<color={g}>{Context.Transform.name}</color>が<color={r}>{_msg}</color>。ご臨終、だな。";
            MessageBroker.Default.Publish(new EventLogMessage()
            {
                Message = msg,
            });
        }
    }
}
