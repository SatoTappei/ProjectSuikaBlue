using System;
using System.Collections.Generic;

namespace UtilityBaseAI
{
    public class AgentTaskHolder
    {
        // TODO: 追加順が必要ない場合はHashSetに変更する
        List<ActionType> _taskList = new(4);
        /// <summary>
        /// TaskListに含まれているかをチェックするためのフラグの配列
        /// 列挙型の各値に対応するデフォルトの数値で管理している
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