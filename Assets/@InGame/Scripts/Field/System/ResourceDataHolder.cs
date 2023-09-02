using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PSB.InGame
{
    public class ResourceDataHolder : MonoBehaviour
    {
        [System.Serializable]
        class Data
        {
            [SerializeField] ResourceType _type;
            [SerializeField] int _value;

            public ResourceType Type => _type;
            public int Value => _value;
        }

        [Header("パラメータに対する資源の回復量")]
        [SerializeField] Data[] _data;

        Dictionary<ResourceType, int> _dict = new();

        void Awake()
        {
            Init();
        }

        void Init()
        {
            _dict = _data.ToDictionary(v => v.Type, v => v.Value);
        }

        public int GetHealingLimit(ResourceType type)
        {
            if (_dict.ContainsKey(type))
            {
                return _dict[type];
            }
            else
            {
                throw new KeyNotFoundException("遷移先のステートが存在しない: " + type);
            }
        }
    }
}