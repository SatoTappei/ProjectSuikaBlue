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
            // とりあえず生成しておく
            _field = _fieldBuilder.Build();
        }
    }
}