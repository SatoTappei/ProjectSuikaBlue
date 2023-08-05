using System.Collections.Generic;
using UnityEngine;

namespace MiniGameECS
{
    public enum EntityType
    {
        // ���o�p
        Debris,
        // �ȉ��_���W�����̃^�C���p
        Grass,
        Wall,
        SpawnPoint,
    }

    public class MonoToEcsTransfer : MonoBehaviour
    {
        public struct Data
        {
            public Vector3 Pos { get; set; }
            public Vector3 Dir { get; set; }
            public EntityType Type { get; set; }
        }

        public static MonoToEcsTransfer Instance { get; private set; }
        Queue<Data> _debrisQueue = new();
        Queue<Data> _tileQueue = new();

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void OnDestroy()
        {
            // static�Ȃ̂ŃL���[�̃N���A���K�v
            _debrisQueue.Clear();
            _tileQueue.Clear();
        }

        // MonoBehavior���ŌĂяo�����\�b�h
        public void AddData(Vector3 pos, Vector3 dir, EntityType type)
        {
            Data data = new Data
            {
                Pos = pos,
                Dir = dir,
                Type = type,
            };

            if (type == EntityType.Debris)
            {
                _debrisQueue.Enqueue(data);
            }
            else
            {
                _tileQueue.Enqueue(data);
            }
        }

        // ECS���ŌĂяo�����\�b�h
        public bool TryGetDebrisData(out Data data) => _debrisQueue.TryDequeue(out data);
        public bool TryGetTileData(out Data data) => _tileQueue.TryDequeue(out data);
    }
}
