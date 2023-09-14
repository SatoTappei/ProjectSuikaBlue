using System.Collections.Generic;
using UnityEngine;

namespace PSB.InGame
{
    public class LeaderSelector : MonoBehaviour
    {
        [SerializeField] float Rate = 1.0f;

        float _timer;

        /// <summary>
        /// ���Ԋu�Ń��[�_�[��I�o����
        /// �X�R�A����ԍ����̂̃��[�_�[�t���O�𗧂Ă�
        /// </summary>
        /// <returns>���̃��[�_�[���I�o:true �I�o�^�C�~���O�ȊO�������͌̂����Ȃ�:false</returns>
        public bool Tick(IReadOnlyList<Actor> candidate, out Actor leader)
        {
            _timer += Time.deltaTime;
            if (_timer > Rate)
            {
                _timer = 0;
                return TrySelect(candidate, out leader);
            }

            leader = null;
            return false;
        }

        bool TrySelect(IReadOnlyList<Actor> candidate, out Actor leader)
        {
            int max = int.MinValue;
            leader = null;
            foreach (Actor actor in candidate)
            {
                // �S�̂̃��[�_�[�t���O��܂�
                actor.IsLeader = false;

                if (actor.Score > max)
                {
                    max = actor.Score;
                    leader = actor;
                }
            }

            // ���̃��[�_�[�̃t���O�𗧂Ă�
            if (leader != null) leader.IsLeader = true;

            return leader != null;
        }
    }
}
