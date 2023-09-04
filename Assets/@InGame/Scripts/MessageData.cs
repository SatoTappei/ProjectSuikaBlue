using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// セルの資源が生成された際にCellクラスから送信される
    /// </summary>
    public struct CellResourceCreateMessage
    {
        public ResourceType Type { get; set; }
        public Vector3 Pos { get; set; }
    }

    /// <summary>
    /// 繁殖ステートで子を生成するタイミングで送信される
    /// 生成に用いるパラメータのみ送信しているので、任意のクラスからも送信可能
    /// 子を生成するスポナーが受信し、子を生成する
    /// </summary>
    public class SpawnChildMessage
    {
        public uint Gene1 { get; set; }
        public uint Gene2 { get; set; }
        public float Food { get; set; }
        public float Water { get; set; }
        public float HP { get; set; }
        public float LifeSpan { get; set; }
        public Vector3 Pos { get; set; }
    }

    /// <summary>
    /// 音を再生する際に送信する
    /// </summary>
    public struct PlayAudioMessage
    {
        public AudioKey Key { get; set; }
    }

    /// <summary>
    /// 各種イベントのログを表示する際に任意のクラスから送信される
    /// </summary>
    public struct EventLogMessage
    {
        public string Message { get; set; }
    }

    /// <summary>
    /// パーティクルの再生を行う際に任意のクラスから送信される
    /// </summary>
    public struct PlayParticleMessage
    {
        public ParticleType Type { get; set; }
        public Vector3 Pos { get; set; }
    }

    /// <summary>
    /// キャラクターが死んだ際に各キャラクターから送信される
    /// </summary>
    public struct ActorDeathMessage
    { 
    }

    // 以下は汎用性など要検証

    /// <summary>
    /// キャラクターが生成された際にスポナーから送信される
    /// エフェクトを表示させるためのエフェクターが受信する
    /// </summary>
    public struct ActorSpawnMessage
    {
        public Vector3 Pos { get; set; }
        public ActionType Type { get; set; }
    }

    /// <summary>
    /// 生成するタイミングで送信される
    /// </summary>
    public struct KurokamiSpawnMessage
    {
        public Vector3 Pos { get; set; }
    }

    /// <summary>
    /// 繁殖する個体が繁殖ステートに遷移したタイミングで送信される
    /// </summary>
    public class BreedingMessage
    {
        public Transform Actor { get; set; }
    }

    /// <summary>
    /// 繁殖する個体が何らかの要因で繁殖をキャンセルするタイミングで送信される
    /// </summary>
    public class CancelBreedingMessage
    {
        public Transform Actor { get; set; }
    }

    /// <summary>
    /// マッチングした際に2人に、それぞれパートナーと性別を渡すために送信される
    /// </summary>
    public class MatchingMessage
    {
        public int ID { get; set; }
        public Transform Partner { get; set; }
        public Sex Sex { get; set; }
    }

    /// <summary>
    /// 繁殖の際、番へ送信/受信されるメッセージ
    /// </summary>
    public struct BreedingPartnerMessage
    {
        public int ID { get; set; }
    }
}