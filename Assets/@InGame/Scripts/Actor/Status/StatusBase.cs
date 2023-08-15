using UnityEngine;

namespace PSB.InGame
{
    [CreateAssetMenu(fileName = "Status_")]
    public class StatusBase : ScriptableObject
    {
        // �S�̂̑S�Ă̒l�̍ő�l��100�ŌŒ�
        public const float Max = 100;

        [SerializeField] ActorType _type;
        [Header("�S�̂̑S�Ă̒l�̍ő�l��100�ŌŒ�\n�ω���(Delta)�Ō��t������")]
        [Range(0, 100)]
        [SerializeField] float _deltaFood;
        [Range(0, 100)]
        [SerializeField] float _deltaWater;
        [Range(0, 100)]
        [SerializeField] float _deltaHp;
        [Range(0, 100)]
        [SerializeField] float _deltaLifeSpan;
        [Range(0, 100)]
        [SerializeField] float _deltaBreedingRate;
        [Range(0.1f, 2.0f)]
        [Header("�ŏ�/�ő�T�C�Y")]
        [SerializeField] float _minSize = 0.5f;
        [Range(0.1f, 2.0f)]
        [SerializeField] float _maxSize = 1.5f;

        public ActorType Type => _type;
        public float DeltaFood => _deltaFood;
        public float DeltaWater => _deltaWater;
        public float DeltaHp => _deltaHp;
        public float DeltaLifeSpan => _deltaLifeSpan;
        public float DeltaBreedingRate => _deltaBreedingRate;
        public float MinSize => _minSize;
        public float MaxSize => _maxSize;
    }
}