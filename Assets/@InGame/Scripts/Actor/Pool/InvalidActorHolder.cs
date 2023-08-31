using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// �L�����N�^�[�̃v�[�����O�����邪�A�C���X�^���X�̐��������s���A���������\�b�h�̌Ăяo���͍s��Ȃ�
    /// �eSpawner�͂��̃N���X����L�����N�^�[�������o���A���������\�b�h���ĂԁB
    /// </summary>
    public class InvalidActorHolder : MonoBehaviour
    {
        /// <summary>
        /// �v�[�����O���鐔
        /// </summary>
        public const int PoolSize = 50;

        [SerializeField] Actor _kinpatsuPrefab;
        [SerializeField] Actor _kurokamiPrefab;

        ActorPool _kinpatsuPool;
        ActorPool _kurokamiPool;

        void Awake()
        {
            CreatePool();
        }

        void CreatePool()
        {
            _kinpatsuPool = new(_kinpatsuPrefab, "KinpatsuPool");
            _kurokamiPool = new(_kurokamiPrefab, "KurokamiPool");

            // UniRx�̃I�u�v�[�̋@�\�Ŕ񓯊��Ńv�[���̒��g�𐶐����Ă���
            _kinpatsuPool.PreloadAsync(PoolSize / 2, 1).Subscribe().AddTo(this);
            _kurokamiPool.PreloadAsync(PoolSize / 2, 1).Subscribe().AddTo(this);
        }

        public Actor Rent(ActorType type)
        {
            if (type == ActorType.Kinpatsu)
            {
                return _kinpatsuPool.Rent();
            }
            else if (type == ActorType.Kurokami)
            {
                return _kurokamiPool.Rent();
            }
            else if (type == ActorType.KinpatsuLeader)
            {
                // TODO:�����v�[������擾���A���[�_�[��p�̏������K�v�H
                return _kinpatsuPool.Rent();
            }
            else
            {
                throw new System.ArgumentException("ActorPool�ɂ�None�ɑΉ��`�L�����N�^�[�͖���");
            }
        }

        void OnDestroy()
        {
            _kinpatsuPool.Dispose();
            _kurokamiPool.Dispose();
            _kinpatsuPool = null;
            _kurokamiPool = null;
        }
    }
}
