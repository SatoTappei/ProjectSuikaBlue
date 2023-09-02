using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// 一定間隔で食事のパーティクル発生のメッセージを送信する
    /// キャラクターの子として生成するアプローチと違い
    /// キャラクターが非アクティブになった瞬間に消えるなどの挙動が防げる
    /// </summary>
    public class EatParticleModule
    {
        // 食事中の演出のパーティクルを出す間隔
        const float PlayParticleRate = 0.5f;

        DataContext _context;
        float _timer;

        public EatParticleModule(DataContext context)
        {
            _context = context;
        }

        public void Reset()
        {
            _timer = PlayParticleRate;
        }

        public void Update()
        {
            _timer += Time.deltaTime;
            if (_timer > PlayParticleRate)
            {
                _timer = 0;
                PlayParticle();
            }
        }

        void PlayParticle()
        {
            MessageBroker.Default.Publish(new PlayParticleMessage()
            {
                Type = ParticleType.Eat,
                Pos = _context.Transform.position,
            });
        }
    }
}