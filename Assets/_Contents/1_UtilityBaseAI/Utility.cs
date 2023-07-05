using System.Collections.Generic;
using UnityEngine;

namespace CommonUtility
{
    /// <summary>
    /// 辞書の便利クラス
    /// </summary>
    public static class DictUtility
    {
        /// <summary>
        /// TryGetValueメソッドをラップしたメソッド
        /// 取得できなかった場合はnullが返る
        /// </summary>
        public static T2 TryGetValue<T1, T2>(Dictionary<T1, T2> dict, T1 key) where T2 : class
        {
            if(dict == null)
            {
                throw new System.NullReferenceException(dict + " 辞書がなるぽ");
            }

            if (dict.TryGetValue(key, out T2 value))
            {
                return value;
            }
            else
            {
                throw new KeyNotFoundException(dict + " 内にキーが無い: " + key);
            }
        }
    }

    /// <summary>
    /// 3Dプリミティブの便利クラス
    /// </summary>
    public static class PrimitiveUtility
    {
        /// <summary>
        /// 3Dオブジェクトの球を作成して返す
        /// </summary>
        public static GameObject CreateSphere(Vector3 pos)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.position = pos;
            return sphere;
        }
    }
}
