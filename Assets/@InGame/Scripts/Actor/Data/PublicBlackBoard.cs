using UnityEngine;

namespace PSB.InGame
{
    public class PublicBlackBoard
    {
        // シングルトン
        static PublicBlackBoard _instance = new();

        Vector3 _gatherPos;
        float _deathRate;

        public static Vector3 GatherPos 
        {
            get => _instance._gatherPos;
            set => _instance._gatherPos = value;
        }
        public static float DeathRate
        {
            get => _instance._deathRate;
            set
            {
                _instance._deathRate = value;
                _instance._deathRate = Mathf.Clamp01(_instance._deathRate);
            }
        }

        /// <summary>
        /// シーンの最後にこのメソッドを呼んで開放すること
        /// </summary>
        public static void Release()
        {
            _instance._gatherPos = default;
            _instance._deathRate = 0;
        }
    }
}
