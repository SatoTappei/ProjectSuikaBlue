using UnityEngine;

namespace UtilityBaseAI
{
    [System.Serializable]
    public class AgentDesire
    {
        public const float MaxValue = 100.0f;

        [Header("‚±‚Ì—~‹‚É‘Î‚·‚és“®‚ğ‹N‚±‚·è‡’l")]
        [Range(0, MaxValue - 1)]
        [SerializeField] int _threshold = 20;
        [Header("Œ¸­‚Ì”{—¦")]
        [Min(0.1f)]
        [SerializeField] float _decreaseMag = 1.0f;

        float _current = MaxValue;

        public float Current => _current;
        public bool BelowThreshold => _current <= _threshold;

        public void Decrease()
        {
            _current -= Time.deltaTime * _decreaseMag;
            Mathf.Clamp(_current, 0, MaxValue);
        }
    }
}