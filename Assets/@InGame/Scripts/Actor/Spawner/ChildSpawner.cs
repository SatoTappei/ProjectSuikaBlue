using UnityEngine;
using UniRx;

namespace PSB.InGame
{
    public class ChildSpawner : ActorSpawner
    {
        [Header("通常時の振れ幅")]
        [SerializeField] byte _randomRange = 2;
        [Header("突然変異率")]
        [SerializeField] float _mutationProb = 0.05f;
        [Header("突然変異した際の振れ幅の倍率")]
        [SerializeField] byte _mutationMag = 3;

        // 突然変異時は 振れ幅の最大値 * 定数倍 だけ変化させる
        byte MutationValue => (byte)(_randomRange * _mutationMag);
        // 通常時は振れ幅のうちランダムな値だけ変化させる
        byte RandomValue => (byte)Random.Range(0, _randomRange + 1);

        void Awake()
        {
            SubscribeReceiveMessage();
        }

        void SubscribeReceiveMessage()
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
            // 最大数に達していたら生成しない
            if (!Check()) return;

            uint childGene = CalcChildGene(msg);
            Actor actor = InstantiateActor(ActorType.Kinpatsu, msg.Pos, childGene);

            // キャラクターを生成したメッセージを送信する
            SendSpawnMessage(actor);
        }

        uint CalcChildGene(SpawnChildMessage msg)
        {
            uint gene1 = msg.Gene1;
            uint gene2 = msg.Gene2;
            IReadOnlyParams paramsRO = msg.Params;

            byte gene1Size = (byte)(gene1 & 0xFF);
            byte gene2Size = (byte)(gene2 & 0xFF);
            byte gene1R = (byte)(gene1 >> 24 & 0xFF);
            byte gene2R = (byte)(gene2 >> 24 & 0xFF);
            byte gene1G = (byte)(gene1 >> 16 & 0xFF);
            byte gene2G = (byte)(gene2 >> 16 & 0xFF);
            byte gene1B = (byte)(gene1 >> 8 & 0xFF);
            byte gene2B = (byte)(gene2 >> 8 & 0xFF);

            // 突然変異したか
            bool isMutation = Random.value < _mutationProb;

            // サイズの設定。平均を取る + 一定確率で突然変異
            int tempSize = (gene1Size + gene2Size) / 2;
            tempSize += (isMutation ? MutationValue : RandomValue) * Sign(paramsRO);
            // byteのサイズにクランプ
            byte size = Clamp(tempSize);

            // カラーの設定。RGBの各値をどちらかの親から取る
            int tempR = Random.Range(0, 2) == 0 ? gene1R : gene2R;
            int tempG = Random.Range(0, 2) == 0 ? gene1G : gene2G;
            int tempB = Random.Range(0, 2) == 0 ? gene1B : gene2B;
            tempR -= (isMutation ? MutationValue : RandomValue) * Sign(paramsRO);
            tempG -= (isMutation ? MutationValue : RandomValue) * Sign(paramsRO);
            tempB -= (isMutation ? MutationValue : RandomValue) * Sign(paramsRO);
            byte r = Clamp(tempR);
            byte g = Clamp(tempG);
            byte b = Clamp(tempB);

            // 音の再生
            AudioManager.PlayAudio(isMutation ? AudioKey.BreedingMutationSE : AudioKey.BreedingSE);

            return (uint)(r << 24 | g << 16 | b << 8 | size);
        }

        /// <summary>
        /// スコアに基づいた確率で、良い変化か悪い変化のどちらに変化するかを決める
        /// </summary>
        /// <returns>良い変化:1 悪い変化:-1</returns>
        int Sign(IReadOnlyParams paramsRO) => Random.value <= CalcScore(paramsRO) ? 1 : -1;

        // TODO:繁殖のスコア計算式がベタ書き
        float CalcScore(IReadOnlyParams paramsRO)
        {
            float score = 5; // それぞれ最大90％

            if (paramsRO.HP >= 0.5f) score++;        // 食料が0.5以上 ++
            if (paramsRO.HP <= 0) score--;           // 食料が0 --
            if (paramsRO.Water >= 0.5f) score++;     // 水分が0.5以上 ++
            if (paramsRO.Water <= 0) score--;        // 水分が0 --
            if (paramsRO.HP >= 0.95f) score++;       // HPが0.95以上 ++
            if (paramsRO.HP <= 0.75f) score--;       // HPが0.75以下 --
            if (paramsRO.LifeSpan >= 0.66f) score++; // 寿命が0.66以上 ++
            if (paramsRO.LifeSpan <= 0.33f) score--; // 寿命が0.33以下 --

            // 0から1の値に成形する
            return score /= 10;
        }

        byte Clamp(int value) => (byte)Mathf.Clamp(value, byte.MinValue, byte.MaxValue);

        void SendSpawnMessage(Actor actor)
        {
            // 生まれた
            MessageBroker.Default.Publish(new ActorSpawnMessage() { Pos = actor.transform.position });
            // ログ
            string color = Utility.ColorCodeGreen;
            string log = $"<color={color}>{actor.name}</color>がこの腐敗した世界に産み落とされたです。";
            MessageBroker.Default.Publish(new EventLogMessage() { Message = log });
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
