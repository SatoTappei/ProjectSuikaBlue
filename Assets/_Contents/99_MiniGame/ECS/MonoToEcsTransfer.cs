using System.Collections.Generic;
using UnityEngine;

namespace MiniGameECS
{
    public enum EntityType
    {
        Debris
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
        Queue<Data> _queue = new();

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
            _queue.Clear();
        }

        /// <summary>
        /// MonoBehavior���ŌĂяo�����\�b�h
        /// �����p�̃f�[�^���L���[�ɋl�߂Ă���
        /// </summary>
        public void AddData(Vector3 pos, Vector3 dir, EntityType type)
        {
            Data data = new Data
            {
                Pos = pos,
                Dir = dir,
                Type = type,
            };
            _queue.Enqueue(data);
        }

        /// <summary>
        /// ECS���ŌĂяo�����\�b�h
        /// �����p�̃f�[�^�̃L���[����1���o��
        /// </summary>
        /// <returns>�f�[�^������: true �f�[�^���Ȃ�: false</returns>
        public bool TryGetData(out Data data) => _queue.TryDequeue(out data);
    }
}
