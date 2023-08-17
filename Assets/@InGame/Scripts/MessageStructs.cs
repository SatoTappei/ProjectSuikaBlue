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
}