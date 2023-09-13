using UnityEngine;

namespace PSB.InGame
{
    public class PublicBlackBoard
    {
        // �V���O���g��
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
        /// �V�[���̍Ō�ɂ��̃��\�b�h���Ă�ŊJ�����邱��
        /// </summary>
        public static void Release()
        {
            _instance._gatherPos = default;
            _instance._deathRate = 0;
        }
    }
}
