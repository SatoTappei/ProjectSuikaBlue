using UnityEngine;

namespace Actor
{
    /// <summary>
    /// ����/�����𐶐�����N���X�͂��̃N���X���p�����邱��
    /// </summary>
    public class ActorSpawner : MonoBehaviour
    {
        static Transform _parent;

        /// <summary>
        /// �������ď��������\�b�h���Ă�ŕԂ�
        /// </summary>
        /// <returns>�������ς݂�Actor</returns>
        protected Actor InstantiateActor(Actor prefab, Vector3 pos, ActorType type)
        {
            Actor actor = Instantiate(prefab, pos, Quaternion.identity);
            if (_parent == null) _parent = new GameObject("ActorParent").transform;
            actor.transform.SetParent(_parent);
            actor.Init(type);

            return actor;
        }

        void OnDestroy()
        {
            if (_parent != null) Destroy(_parent);
        }
    }
}
