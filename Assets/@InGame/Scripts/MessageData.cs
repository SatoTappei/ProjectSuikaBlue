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

    /// <summary>
    /// �ɐB����̂��ɐB�X�e�[�g�ɑJ�ڂ����^�C�~���O�ő��M�����
    /// </summary>
    public class BreedingMessage
    {
        public Transform Actor { get; set; }
    }

    /// <summary>
    /// �ɐB����̂����炩�̗v���ŔɐB���L�����Z������^�C�~���O�ő��M�����
    /// </summary>
    public class CancelBreedingMessage
    {
        public Transform Actor { get; set; }
    }

    /// <summary>
    /// �}�b�`���O�����ۂ�2�l�ɁA���ꂼ��p�[�g�i�[�Ɛ��ʂ�n�����߂ɑ��M�����
    /// </summary>
    public class MatchingMessage
    {
        public int ID { get; set; }
        public Transform Partner { get; set; }
        public Sex Sex { get; set; }
    }

    /// <summary>
    /// �ɐB�̍ہA�Ԃ֑��M/��M����郁�b�Z�[�W
    /// </summary>
    public struct BreedingPartnerMessage
    {
        public int ID { get; set; }
    }

    /// <summary>
    /// �ɐB�X�e�[�g�Ŏq�𐶐�����^�C�~���O�ŃR�[���o�b�N���瑗�M�����
    /// �q�𐶐�����N���X����M����
    /// </summary>
    public class SpawnChildMessage
    {
        public uint Gene1 { get; set; }
        public uint Gene2 { get; set; }
        public IReadOnlyParams Params { get; set; }
        public Vector3 Pos { get; set; }
    }

    /// <summary>
    /// �L�����N�^�[�����񂾍ۂɊe�L�����N�^�[���瑗�M�����
    /// </summary>
    public struct ActorDeathMessage
    {
        public Vector3 Pos { get; set; }
        public ActionType Type { get; set; }
    }
}