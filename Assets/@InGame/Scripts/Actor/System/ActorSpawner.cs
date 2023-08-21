using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// ����/�����𐶐�����N���X�͂��̃N���X���p�����邱��
    /// </summary>
    public class ActorSpawner : MonoBehaviour
    {
        static Transform _parent;

        /// <summary>
        /// �������Đe�̈�`�q���q�ɓn�����������\�b�h���Ă�ŕԂ��B
        /// ��O��������null�̏ꍇ�A��`�q�������̂Őe�����ɂȂ�B
        /// </summary>
        /// <returns>�������ς݂�Actor</returns>
        protected Actor InstantiateActor(Actor prefab, Vector3 pos, uint? gene = null)
        {
            if (_parent == null) _parent = new GameObject("ActorParent").transform;

            Actor actor = Instantiate(prefab, pos, Quaternion.identity, _parent);
            actor.Init(gene);
            // �����_���Ȗ��O
            actor.gameObject.name = Utility.GetRandomString();

            return actor;
        }

        void OnDestroy()
        {
            if (_parent != null) Destroy(_parent);
        }
    }
}
