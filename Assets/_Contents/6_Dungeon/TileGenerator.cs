using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// �����ɑΉ������_���W�����̊e�^�C���𐶐�����N���X
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

    [Header("�����ƑΉ�����Prefab�̃f�[�^")]
    [SerializeField] TileData[] _data;
    Dictionary<char, GameObject> _tileDataDict = new();

    void Awake()
    {
        _tileDataDict = _data.ToDictionary(v => v.Letter, v => v.Prefab);
    }

    /// <summary>
    /// ��������O�ɑΉ������������ǂ������`�F�b�N
    /// </summary>
    public bool IsSupported(char letter) => _tileDataDict.ContainsKey(letter);

    /// <summary>
    /// �����ɑΉ������_���W�����̃^�C���𐶐����ĕԂ�
    /// </summary>
    public GameObject Generate(char letter)
    {
        if (_tileDataDict.TryGetValue(letter, out GameObject prefab))
        {
            return Instantiate(prefab, Vector3.zero, Quaternion.identity);
        }
        else
        {
            throw new System.ArgumentException("�_���W���������p�̕������Ή����Ă��Ȃ�: " + letter);
        }
    }
}
