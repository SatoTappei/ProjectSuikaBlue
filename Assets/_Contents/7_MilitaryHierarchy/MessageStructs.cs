using UnityEngine;

namespace MilitaryHierarchy
{
    /// <summary>
    /// 部隊長から各兵士への命令に使用する列挙型
    /// </summary>
    public enum TankOrder
    {
        None,
        Follow,
        Fire,
    }

    /// <summary>
    /// 将軍が部隊長に移動先を命令するメッセージ
    /// </summary>
    public struct DestinationMessage
    {
        public Vector3 Destination { get; set; }
        public int TroopNum { get; set; }
    }

    /// <summary>
    /// 部隊長から各兵士への命令に使用するメッセージ
    /// </summary>
    public struct TankOrderMessage
    {
        public TankOrder Order { get; set; }
        public int TroopNum { get; set; }
    }
}