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