using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileGenerator : MonoBehaviour
{
    [System.Serializable]
    class TileData
    {
        [SerializeField] char _letter;
        [SerializeField] GameObject _prefab;

        public char Letter => _letter;
        public GameObject Prefab => _prefab;
    }

    [Header("•¶Žš‚Æ‘Î‰ž‚·‚éƒvƒŒƒnƒu")]
    [SerializeField] TileData[] _data;
    Dictionary<char, GameObject> _tileDataDict = new();

    void Awake()
    {
        _tileDataDict = _data.ToDictionary(v => v.Letter, v => v.Prefab);
    }

    //public GameObject Generate(char letter)
    //{
    //    if (_tileDataDict.TryGetValue(letter, out GameObject prefab))
    //    {
    //        return Instantiate(prefab,Vector3.zero,Quaternion.identity)
    //    }
    //    else
    //    {

    //    }
    //}
}
