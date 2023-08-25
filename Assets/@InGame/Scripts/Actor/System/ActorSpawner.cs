using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// ����/�����𐶐�����N���X�͂��̃N���X���p�����邱��
    /// </summary>
    public class ActorSpawner : MonoBehaviour
    {
        const int Max = 50;
        static int _count;
        static Transform _parent;

        void Awake()
        {
            ReceiveMessage();
        }

        void ReceiveMessage()
        {
            ActorDeathReceiver receiver = new(_ => _count--, gameObject);
        }

        /// <summary>
        /// �������Đe�̈�`�q���q�ɓn�����������\�b�h���Ă�ŕԂ��B
        /// ��O��������null�̏ꍇ�A��`�q�������̂Őe�����ɂȂ�B
        /// </summary>
        /// <returns>�ʏ�:�������ς݂�Actor �ő吔�����ς݂̏ꍇ:null</returns>
        protected Actor InstantiateActor(Actor prefab, Vector3 pos, uint? gene = null)
        {
            // �������𐔂���
            _count++;

            if (_parent == null) _parent = new GameObject("ActorParent").transform;

            Actor actor = Instantiate(prefab, pos, Quaternion.identity, _parent);
            actor.Init(gene);
            // �����_���Ȗ��O
            actor.gameObject.name = Utility.GetRandomString();

            return actor;
        }

        protected bool Check()
        {
            if (_count >= Max)
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