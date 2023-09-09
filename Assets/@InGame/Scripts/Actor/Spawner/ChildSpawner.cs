using UnityEngine;
using UniRx;

namespace PSB.InGame
{
    /// <summary>
    /// 生成に必要なパラメータのメッセージを受信して子を生成する
    /// </summary>
    public class ChildSpawner : ActorSpawner
    {
        enum Result
        {
            Normal,
            Mutation,
        }

        [Header("通常時の振れ幅")]
        [SerializeField] byte _randomRange = 2;
        [Header("突然変異した際の振れ幅の倍率")]
        [SerializeField] byte _mutationMag = 3;
        [Header("突然変異率")]
        [Range(0, 1)]
        [SerializeField] float _mutationProb = 0.05f;
        [Header("スコアを計算する際の食料の閾値")]
        [Range(0, 1)]
        [SerializeField] float _foodThresholdHigh = 0.5f;
        [Range(0, 1)]
        [SerializeField] float _foodThresholdLow = 0;
        [Header("スコアを計算する際の水分の閾値")]
        [Range(0, 1)]
        [SerializeField] float _waterThresholdHigh = 0.5f;
        [Range(0, 1)]
        [SerializeField] float _waterThresholdLow = 0;
        [Header("スコアを計算する際の体力の閾値")]
        [Range(0, 1)]
        [SerializeField] float _hpThresholdHigh = 0.9f;
        [Range(0, 1)]
        [SerializeField] float _hpThresholdLow = 0.6f;
        [Header("スコアを計算する際の寿命の閾値")]
        [Range(0, 1)]
        [SerializeField] float _lifeSpanThresholdHigh = 0.66f;
        [Range(0, 1)]
        [SerializeField] float _lifeSpanThresholdLow = 0.33f;

        // 突然変異時は 振れ幅の最大値 * 定数倍 だけ変化させる
        byte MutationValue => (byte)(_randomRange * _mutationMag);
        // 通常時は振れ幅のうちランダムな値だけ変化させる
        byte RandomValue => (byte)Random.Range(0, _randomRange + 1);

        void Awake()
        {
            ReceiveMessage();
        }

        void ReceiveMessage()
        {
            MessageBroker.Default.Receive<SpawnChildMessage>().Subscribe(Execute).AddTo(this);
        }

        /// <summary>
        /// 子の生成を行う
        /// 両親の遺伝子 + 雌のステータス を基に子の遺伝子が決定する
        /// サイズは大きい方、色は濃い方に遺伝子が良い
        /// </summary>
        void Execute(SpawnChildMessage msg)
        {
            Result result = CalcChildGene(msg, out uint childGene);

            if (TryInstantiate(ActorType.Kinpatsu, msg.Pos, out Actor actor, childGene))
            {
                SendEventLogMessage(actor, result);
                PlaySE(result);
            }
        }

        Result CalcChildGene(SpawnChildMessage msg, out uint gene)
        {
            uint gene1 = msg.Gene1;
            uint gene2 = msg.Gene2;

            byte gene1Size = (byte)(gene1 & 0xFF);
            byte gene2Size = (byte)(gene2 & 0xFF);
            byte gene1R = (byte)(gene1 >> 24 & 0xFF);
            byte gene2R = (byte)(gene2 >> 24 & 0xFF);
            byte gene1G = (byte)(gene1 >> 16 & 0xFF);
            byte gene2G = (byte)(gene2 >> 16 & 0xFF);
            byte gene1B = (byte)(gene1 >> 8 & 0xFF);
            byte gene2B = (byte)(gene2 >> 8 & 0xFF);

            // スコアの計算
            float score = CalcScore(msg);

            // 突然変異したか
            bool isMutation = Random.value < _mutationProb;

            // サイズの設定。平均を取る + 一定確率で突然変異
            int tempSize = (gene1Size + gene2Size) / 2;
            tempSize += SubmitValue(isMutation) * Sign(score);
            // byteのサイズにクランプ
            byte size = Clamp(tempSize);

            // カラーの設定。RGBの各値をどちらかの親から取る
            int tempR = NextBool() ? gene1R : gene2R;
            int tempG = NextBool() ? gene1G : gene2G;
            int tempB = NextBool() ? gene1B : gene2B;
            tempR -= SubmitValue(isMutation) * Sign(score);
            tempG -= SubmitValue(isMutation) * Sign(score);
            tempB -= SubmitValue(isMutation) * Sign(score);
            byte r = Clamp(tempR);
            byte g = Clamp(tempG);
            byte b = Clamp(tempB);

            gene = (uint)(r << 24 | g << 16 | b << 8 | size);
            return isMutation ? Result.Mutation : Result.Normal;
        }

        float CalcScore(SpawnChildMessage msg)
        {
            // 最後に10で割り、0~1の間に成型する
            // それぞれ最大90％
            float score = 5;

            if (msg.Food >= _foodThresholdHigh) score++;         // 食料がn以上 ++
            if (msg.Food <= _foodThresholdLow) score--;          // 食料がn以下 --
            if (msg.Water >= _waterThresholdHigh) score++;       // 水分がn以上 ++
            if (msg.Water <= _waterThresholdLow) score--;        // 水分がn以下 --
            if (msg.HP >= _hpThresholdHigh) score++;             // HPがn以上 ++
            if (msg.HP <= _hpThresholdLow) score--;              // HPがn以下 --
            if (msg.LifeSpan >= _lifeSpanThresholdHigh) score++; // 寿命がn以上 ++
            if (msg.LifeSpan <= _lifeSpanThresholdLow) score--;  // 寿命がn以下 --

            // 0から1の値に成形する
            return score /= 10;
        }

        byte SubmitValue(bool isMutation) => isMutation ? MutationValue : RandomValue;
        int Sign(float score) => Random.value <= score ? 1 : -1;
        bool NextBool() => Random.Range(0, 2) == 0;
        byte Clamp(int value) => (byte)Mathf.Clamp(value, byte.MinValue, byte.MaxValue);

        void SendEventLogMessage(Actor actor, Result result)
        {
            string color = Utility.ColorCodeGreen;
            string log = string.Empty;
            if (result == Result.Normal)
            {
                log = $"<color={color}>{actor.name}</color>がこの腐敗した世界に産み落とされたです。";
            }
            else if (result == Result.Mutation)
            {
                log = $"<color={color}>{actor.name}</color>がこの腐敗した世界に産み落とされたです。突然変異！";
            }
            MessageBroker.Default.Publish(new EventLogMessage() { Message = log });
        }

        void PlaySE(Result result)
        {
            // 音の再生
            AudioManager.PlayAudio(result == Result.Mutation ? AudioKey.BreedingMutationSE : AudioKey.BreedingSE);
        }

        // デバッグ用
        void Log(uint gene, float sizeMax = 1.5f, float sizeMin = 0.5f)
        {
            byte colorR = (byte)(gene >> 24 & 0xFF);
            byte colorG = (byte)(gene >> 16 & 0xFF);
            byte colorB = (byte)(gene >> 8 & 0xFF);

            // 0~255
            float f = gene & 0xFF;
            // fを最小/最大サイズの範囲にリマップ
            float size = (f - 0) * (sizeMax - sizeMin) / (byte.MaxValue - byte.MinValue) + sizeMin;

            Debug.Log($"R:{colorR} G:{colorG} B:{colorB} サイズ:{size}");
        }
    }
}
