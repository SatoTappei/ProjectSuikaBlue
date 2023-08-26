using System;
using UnityEngine;

namespace PSB.InGame
{
    public static class Utility
    {
        const string CharTable = "abcdefghijklmnopqrstuvwxyz1234567890";

        public const string ColorCodeGreen = "#21ff37";
        public const string ColorCodeRed = "#ff6759";

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

        /// <summary>
        /// �����_���ȕ�������擾
        /// </summary>
        /// <returns>length������a~z��������1~9 �ō\�����ꂽ������</returns>
        public static string GetRandomString(int length = 8)
        {
            string s = string.Empty;
            for(int i = 0; i < length; i++)
            {
                int r = UnityEngine.Random.Range(0, CharTable.Length);
                s += CharTable[r];
            }

            return s;
        }
    }
}