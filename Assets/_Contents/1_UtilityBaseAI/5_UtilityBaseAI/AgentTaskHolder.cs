using System;
using System.Collections.Generic;

namespace UtilityBaseAI
{
    public class AgentTaskHolder
    {
        // TODO: �ǉ������K�v�Ȃ��ꍇ��HashSet�ɕύX����
        List<ActionType> _taskList = new(4);
        /// <summary>
        /// TaskList�Ɋ܂܂�Ă��邩���`�F�b�N���邽�߂̃t���O�̔z��
        /// �񋓌^�̊e�l�ɑΉ�����f�t�H���g�̐��l�ŊǗ����Ă���
        /// </summary>
        bool[] _containFlags = new bool[Enum.GetValues(typeof(ActionType)).Length];

        public void Add(ActionType type)
        {
            _taskList.Add(type);
            _containFlags[(int)type] = true;
        }

        public bool IsContain(ActionType type) => _containFlags[(int)type];
    }
}