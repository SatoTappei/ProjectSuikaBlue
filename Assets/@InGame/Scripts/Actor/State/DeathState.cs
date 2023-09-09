using UniRx;

namespace PSB.InGame
{
    /// <summary>
    /// 死亡した際にコライダーとレンダラーを無効化。
    /// パーティクルの発生、ログに表示するメッセージを送信する。
    /// </summary>
    public class DeathState : BaseState
    {
        readonly FieldModule _field;
        readonly ParticleType _particleType;
        readonly string _msg;

        public DeathState(DataContext context, StateType stateType, ParticleType particleType, string msg) 
            : base(context, stateType)
        {
            _field = new(context);
            _particleType = particleType;
            _msg = msg;
        }

        protected override void Enter()
        {
        }

        protected override void Exit()
        {
        }

        protected override void Stay()
        {
            // 死亡した際にはこれ以上遷移しないためセルの予約をしない。

            Invalid();
            SendParticleMessage();
            SendLogMessage();
        }

        void Invalid()
        {
            _field.DeleteActorOnCell();
            Context.ReturnToPool?.Invoke();
            // Enterのタイミングでプールに戻すので、次に取り出した際にEnterから始まるようにリセットする
            ResetStage();

            MessageBroker.Default.Publish(new ActorDeathMessage() { Type = Context.Type });
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
