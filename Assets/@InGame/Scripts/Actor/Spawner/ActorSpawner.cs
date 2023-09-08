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
            MessageBroker.Default.Receive<ActorDeathMessage>().Subscribe(_ => _count--).AddTo(this);
        }

        /// <summary>
        /// �������Đe�̈�`�q���q�ɓn�����������\�b�h���Ă�ŕԂ��B
        /// ��O��������null�̏ꍇ�A��`�q�������̂Őe�����ɂȂ�B
        /// </summary>
        /// <returns>��������:true �ő吔�ɒB���Ă��萶���s�\:false</returns>
        protected bool TryInstantiate(ActorType type, Vector3 pos, out Actor actor, uint? gene = null)
        {
            if (_parent == null) _parent = new GameObject("ActorParent").transform;

            if (CheckSpawn())
            {
                _count++;

                actor = _holder.Rent(type);
                actor.transform.position = pos;
                actor.transform.rotation = Quaternion.identity;
                actor.transform.SetParent(_parent);
                // �����_���Ȗ��O
                actor.name = Utility.GetRandomString();

                actor.Init(gene);

                return true;
            }

            actor = null;
            return false;
        }

        /// <summary>
        /// �����\���ǂ���
        /// </summary>
        /// <returns>�����\:true �������:false</returns>
        bool CheckSpawn()
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