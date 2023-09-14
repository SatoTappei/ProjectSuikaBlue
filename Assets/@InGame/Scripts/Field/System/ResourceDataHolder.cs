using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PSB.InGame
{
    public class ResourceDataHolder : MonoBehaviour
    {
        [System.Serializable]
        public class Data
        {
            [SerializeField] ResourceType _type;
            [SerializeField] int _healingLimit = 100;

            public ResourceType Type => _type;
            public int HealingLimit => _healingLimit;
        }

        [Header("ƒpƒ‰ƒ[ƒ^‚É‘Î‚·‚é‘Œ¹‚Ì‰ñ•œ—Ê")]
        [SerializeField] Data[] _data;

        Dictionary<ResourceType, Data> _dict = new();

        public Data this[ResourceType type] => _dict[type];

        void Awake()
        {
            _dict = _data.ToDictionary(v => v.Type, v => v);
        }
    }
}