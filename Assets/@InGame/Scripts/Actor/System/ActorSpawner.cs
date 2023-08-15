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
        /// �������ď��������\�b�h���Ă�ŕԂ�
        /// ���������ɐe�̈�`�q���q�ɓn���K�v������
        /// </summary>
        /// <returns>�������ς݂�Actor</returns>
        protected Actor InstantiateActor(Actor prefab, Vector3 pos, int gene = Actor.DefaultGene)
        {
            _parent ??= new GameObject("ActorParent").transform;

            Actor actor = Instantiate(prefab, pos, Quaternion.identity, _parent);
            actor.InitOnStart(gene);
            
            //if (_parent == null) _parent = new GameObject("ActorParent").transform;
            
            //actor.transform.SetParent(_parent);

            return actor;
        }

        void OnDestroy()
        {
            if (_parent != null) Destroy(_parent);
        }
    }
}
