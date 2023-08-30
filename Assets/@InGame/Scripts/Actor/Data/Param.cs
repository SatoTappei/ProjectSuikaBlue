using UnityEngine;

namespace PSB.InGame
{
    public struct Param
    {
        float _value;

        public Param(float value)
        {
            _value = value;
        }

        public float Value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
                _value = Mathf.Clamp(_value, 0, StatusBase.Max);
            }
        }

        public float Max => StatusBase.Max;
        public float Percentage => Value / StatusBase.Max;
        public bool IsBelowZero => Value <= 0;
    }
}
