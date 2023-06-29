using System.Collections.Generic;
using UnityEngine;

namespace UtilityBaseAI
{
    public class InteractObjectManager : MonoBehaviour
    {
        [System.Serializable]
        struct InteractObject
        {
            [SerializeField] Transform _object;
            [SerializeField] ActionType _type;

            public Transform Object => _object;
            public ActionType Type => _type;
        }

        [SerializeField] InteractObject[] _objects;

        Dictionary<ActionType, List<Transform>> _ObjectPosDict = new();

        void Awake()
        {
            foreach(InteractObject obj in _objects)
            {
                List<Transform> list = _ObjectPosDict[obj.Type];
                if(list == null)
                {
                    list = new();
                }

                list.Add(obj.Object);
            }
        }
    }
}
