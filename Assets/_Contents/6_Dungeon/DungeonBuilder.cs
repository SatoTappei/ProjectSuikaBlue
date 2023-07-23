using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 文字列からダンジョンを生成するクラス
/// テキストファイルもしくはアルゴリズムで生成した文字列を使用する
/// </summary>
[RequireComponent(typeof(TileGenerator))]
public class DungeonBuilder : MonoBehaviour
{
    enum BuildMode
    {
        TextAsset,
        Algorithm,
    }

    [Header("ダンジョンの親オブジェクト")]
    [SerializeField] Transform _parent;
    [Header("テキストファイル/アルゴリズム")]
    [SerializeField] BuildMode _buildMode;
    [Header("文字列テキスト")]
    [SerializeField] TextAsset _blueprint;

    void Start()
    {
        if (_buildMode == BuildMode.TextAsset) BuildFromTextAsset();
        if (_buildMode == BuildMode.Algorithm) BuildFromAlgorithm();
    }

    /// <summary>
    /// インスペクターに割り当てられたテキストファイルからダンジョンを生成する
    /// </summary>
    void BuildFromTextAsset()
    {
        string[] lines = _blueprint.text.Split("\n");
        // テキストの場合、文字列の前後の空白を削除しないとズレる
        for(int i = 0; i < lines.Length; i++)
        {
            lines[i] = lines[i].Trim();
        }
        Generate(lines);
    }

    /// <summary>
    /// アルゴリズムを用いて生成した文字列からダンジョンを生成する
    /// </summary>
    void BuildFromAlgorithm()
    {
        if(TryGetComponent(out IDungeonBaseStringGenerator stringGenerator))
        {
            string[] lines = stringGenerator.Generate();
            Generate(lines);
        }
        else
        {
            throw new System.NullReferenceException("ダンジョン生成するアルゴリズムが無い");
        }
    }

    /// <summary>
    /// 文字列を解析して各文字に対応したタイルを生成、適切な位置に配置する
    /// </summary>
    void Generate(string[] lines)
    {
        TileGenerator tileGenerator = GetComponent<TileGenerator>();

        for (int i = 0; i < lines.Length; i++)
        {
            // 中心を基準
            float basePosX = _parent.position.x - lines[i].Length / 2;
            float basePosZ = _parent.position.z - lines.Length / 2;

            for (int k = 0; k < lines[i].Length; k++)
            {
                if (!tileGenerator.IsSupported(lines[i][k])) continue;

                // 生成して位置と親の設定
                GameObject tile = tileGenerator.Generate(lines[i][k]);
                tile.transform.position = new Vector3(basePosX + k, 0, basePosZ + i);
                tile.transform.SetParent(_parent);
            }
        }
    }
}
