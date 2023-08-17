using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// �Z���̎������������ꂽ�ۂ�Cell�N���X���瑗�M�����
    /// </summary>
    public struct CellResourceCreateMessage
    {
        public ResourceType Type { get; set; }
        public Vector3 Pos { get; set; }
    }

    /// <summary>
    /// ��������^�C�~���O�ő��M�����
    /// </summary>
    public struct KurokamiSpawnMessage
    {
        public Vector3 Pos { get; set; }
    }
}