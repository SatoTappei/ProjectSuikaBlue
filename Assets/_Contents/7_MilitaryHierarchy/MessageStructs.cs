using UnityEngine;

namespace MilitaryHierarchy
{
    /// <summary>
    /// ����������e���m�ւ̖��߂Ɏg�p����񋓌^
    /// </summary>
    public enum TankOrder
    {
        None,
        Follow,
        Fire,
    }

    /// <summary>
    /// ���R���������Ɉړ���𖽗߂��郁�b�Z�[�W
    /// </summary>
    public struct DestinationMessage
    {
        public Vector3 Destination { get; set; }
        public int TroopNum { get; set; }
    }

    /// <summary>
    /// ����������e���m�ւ̖��߂Ɏg�p���郁�b�Z�[�W
    /// </summary>
    public struct TankOrderMessage
    {
        public TankOrder Order { get; set; }
        public int TroopNum { get; set; }
    }
}