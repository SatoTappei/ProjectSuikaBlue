using System;
using UnityEngine;

namespace PSB.InGame
{
    public class Utility
    {
        /// <summary>
        /// ���͔��ߖT���w�肷��p�̕����̔z��
        /// </summary>
        public static readonly Vector2Int[] EightDirections =
        {
            new Vector2Int(-1, 0),
            new Vector2Int(-1, 1),
            new Vector2Int(0, 1),
            new Vector2Int(1, 1),
            new Vector2Int(1, 0),
            new Vector2Int(1, -1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, -1),
        };

        /// <summary>
        /// �񋓌^�̗v�f�����擾
        /// </summary>
        /// <returns>�v�f��</returns>
        public static int GetEnumLength<T>() where T : Enum => Enum.GetValues(typeof(T)).Length;
    }
}