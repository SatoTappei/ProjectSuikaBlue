using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 文字に対応したダンジョンの各タイルを生成するクラス
/// </summary>
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

    [Header("文字と対応するPrefabのデータ")]
    [SerializeField] TileData[] _data;
    Dictionary<char, GameObject> _tileDataDict = new();

    void Awake()
    {
        _tileDataDict = _data.ToDictionary(v => v.Letter, v => v.Prefab);
    }

    /// <summary>
    /// 生成する前に対応した文字かどうかをチェック
    /// </summary>
    public bool IsSupported(char letter) => _tileDataDict.ContainsKey(letter);

    /// <summary>
    /// 文字に対応したダンジョンのタイルを生成して返す
    /// </summary>
    public GameObject Generate(char letter)
    {
        if (_tileDataDict.TryGetValue(letter, out GameObject prefab))
        {
            return Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            throw new System.ArgumentException("ダンジョン生成用の文字が対応していない: " + letter);
        }
    }
}
