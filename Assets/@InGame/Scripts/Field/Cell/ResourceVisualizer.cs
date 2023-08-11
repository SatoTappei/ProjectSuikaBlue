using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace PSB.InGame
{
    public class ResourceVisualizer : MonoBehaviour
    {
        [System.Serializable]
        class Data
        {
            public GameObject _prefab;
            public ResourceType _type;
        }

        [SerializeField] Data[] _data;

        Dictionary<ResourceType, GameObject> _prefabDict = new();
        Dictionary<Vector3, GameObject> _objectDict = new();
        Transform _parent;

        void Awake()
        {
            CreateResourceParent();
            CreateDict();
            SettingMessageReceive();
        }

        void CreateResourceParent()
        {
            _parent = new GameObject("ResourceObjectParent").transform;
        }

        void CreateDict()
        {
            foreach (Data data in _data)
            {
                _prefabDict.Add(data._type, data._prefab);
            }
        }

        /// <summary>
        /// セルが資源生成時に送信するメッセージを受信して対応するオブジェクトを生成する
        /// セルは値を、このクラスがオブジェクトをそれぞれ管理し、互いを知ることは無い
        /// </summary>
        void SettingMessageReceive()
        {
            MessageBroker.Default.Receive<CellResourceCreateMessage>().Subscribe(msg =>
            {
                Destroy(msg.Pos);
                if (msg.Type != ResourceType.None) Instantiate(msg.Type, msg.Pos);
            }).AddTo(this);
        }

        void Destroy(Vector3 key)
        {
            if (_objectDict.TryGetValue(key, out GameObject value))
            {
                Destroy(value);
            }
        }

        void Instantiate(ResourceType key, Vector3 pos)
        {
            if(_prefabDict.TryGetValue(key,out GameObject value))
            {
                GameObject go = Instantiate(value, pos, Quaternion.identity, _parent);
                if (_objectDict.ContainsKey(pos))
                {
                    _objectDict[pos] = go;
                }
                else
                {
                    _objectDict.Add(pos, go);
                }
            }
            else
            {
                throw new KeyNotFoundException("資源のPrefabが辞書に登録されていない: " + key);
            }
        }
    }
}