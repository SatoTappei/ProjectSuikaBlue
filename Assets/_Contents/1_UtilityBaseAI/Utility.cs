using System.Collections.Generic;

namespace CommonUtility
{
    /// <summary>
    /// �����֗̕��N���X
    /// </summary>
    public static class DictUtility
    {
        /// <summary>
        /// TryGetValue���\�b�h�����b�v�������\�b�h
        /// �擾�ł��Ȃ������ꍇ��null���Ԃ�
        /// </summary>
        public static T2 TryGetValue<T1, T2>(Dictionary<T1, T2> dict, T1 key) where T2 : class
        {
            if(dict == null)
            {
                throw new System.NullReferenceException(dict + " �������Ȃ��");
            }

            if (dict.TryGetValue(key, out T2 value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException(dict + " ���ɃL�[������: " + key);
            }
        }
    }
}
