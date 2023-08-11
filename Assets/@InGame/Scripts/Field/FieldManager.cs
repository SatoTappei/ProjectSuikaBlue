using UnityEngine;

namespace PSB.InGame
{
    public class FieldManager : MonoBehaviour
    {
        [SerializeField] FieldBuilder _fieldBuilder;

        Cell[,] _field;

        public Cell[,] Field
        {
            get
            {
                _field ??= _fieldBuilder.Build();
                return _field;
            }
        }

        void Start()
        {
            // Ç∆ÇËÇ†Ç¶Ç∏ê∂ê¨ÇµÇƒÇ®Ç≠
            _field = _fieldBuilder.Build();
        }
    }
}