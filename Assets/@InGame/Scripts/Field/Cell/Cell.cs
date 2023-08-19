using UnityEngine;
using UniRx;

namespace PSB.InGame
{
    public enum TileType
    {
        Water,
        Soil,
        Grass,
    }

    public enum ResourceType
    {
        None,
        PalmTree,
        Tree,
        Stone,
        Warter,
    }

    public class Cell
    {
        TileType _tileType;
        ResourceType _resourceType;
        ActorType _actorType;
        Vector3 _pos;
        int _height;

        public Cell(TileType type, Vector3 pos, float height)
        {
            _tileType = type;
            _pos = pos;
            // �p�[�����m�C�Y�œ���ꂽ�Z���̍����������_�ȉ��Ȃ̂�1000�{���Đ����ɐ��`����
            _height = (int)(height * 1000);
        }

        public TileType TileType => _tileType;
        public Vector3 Pos => _pos;
        public int Height => _height;
        public bool IsWalkable => _tileType != TileType.Water && _resourceType == ResourceType.None;
        public bool IsEmpty => IsWalkable && _actorType == ActorType.None;

        public ResourceType ResourceType
        {
            get { return _resourceType; }
            set { _resourceType = value; SendResourceCreateMessage(_resourceType); }
        }
        public ActorType ActorType
        {
            get => _actorType;
            set => _actorType = value;
        }

        /// <summary>
        /// �Z���Ɏ����𐶐������ۂɁA�����ǂ̍��W�ɐ����������̃��b�Z�[�W���O���s��
        /// </summary>
        void SendResourceCreateMessage(ResourceType type)
        {
            MessageBroker.Default.Publish(new CellResourceCreateMessage()
            {
                Type = type,
                Pos = Pos,
            });
        }
    }
}

