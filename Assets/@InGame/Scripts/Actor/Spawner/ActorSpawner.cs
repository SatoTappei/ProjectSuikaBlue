using UnityEngine;
using UniRx;

namespace PSB.InGame
{
    /// <summary>
    /// ����/�����𐶐�����N���X�͂��̃N���X���p�����邱��
    /// </summary>
    public class ActorSpawner : MonoBehaviour
    {
        static int _count;
        static Transform _parent;

        [SerializeField] InvalidActorHolder _holder;

        void Awake()
        {
            ReceiveMessage();
        }

        void ReceiveMessage()
        {
            // ���񂾃��b�Z�[�W����M�����ۂɐ�������1���炷
            MessageBroker.Default.Receive<ActorDeathMessage>()
                .Where(msg => msg.Type == ActionType.Killed || msg.Type == ActionType.Senility)
                .Subscribe(_ => _count--).AddTo(this);
        }

        /// <summary>
        /// �������Đe�̈�`�q���q�ɓn�����������\�b�h���Ă�ŕԂ��B
        /// ��O��������null�̏ꍇ�A��`�q�������̂Őe�����ɂȂ�B
        /// </summary>
        /// <returns>�ʏ�:�������ς݂�Actor �ő吔�����ς݂̏ꍇ:null</returns>
        protected Actor InstantiateActor(ActorType type, Vector3 pos, uint? gene = null)
        {
            // �������𑝂₷
            _count++;

            if (_parent == null) _parent = new GameObject("ActorParent").transform;

            Actor actor = _holder.Rent(type);
            actor.transform.position = pos;
            actor.transform.rotation = Quaternion.identity;
            actor.transform.SetParent(_parent);
            actor.Init(gene);
            // �����_���Ȗ��O
            actor.name = Utility.GetRandomString();

            return actor;
        }

        /// <summary>
        /// �������𒲂ׂ����ꍇ�ɌĂ�
        /// </summary>
        /// <returns>�����\:true �������:false</returns>
        protected bool CheckSpawn()
        {
            if (_count >= InvalidActorHolder.PoolSize)
            {
                Debug.LogWarning("�L�����N�^�[�̐��������ő�ɒB���Ă���̂ŔɐB�s�\");
                return false;
            }

            return true;
        }

        void OnDestroy()
        {
            if (_parent != null) Destroy(_parent);
            _count = 0;
        }
    }
}