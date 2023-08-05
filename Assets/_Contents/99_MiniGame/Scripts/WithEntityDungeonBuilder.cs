using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using MiniGameECS;

namespace MiniGame
{
    /// <summary>
    /// DungeonBuilderのほぼコピペ
    /// 生成したタイルと箇所をECS側に伝える処理が追加されただけ
    /// </summary>
    [RequireComponent(typeof(TileGenerator))]
    public class WithEntityDungeonBuilder : MonoBehaviour
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

        /// <summary>
        /// 文字に対応したタイルのリストを取得するための辞書
        /// </summary>
        Dictionary<char, List<GameObject>> _tileDataDict = new();
        /// <summary>
        /// 既にダンジョンが生成済みの場合に呼び出していないかチェックするためのフラグ
        /// </summary>
        bool _created;

        public IReadOnlyDictionary<char, List<GameObject>> TileDataDict => _tileDataDict;

        /// <summary>
        /// 外部から呼び出すことでダンジョンを生成する
        /// </summary>
        public void Build()
        {
            if (_created)
            {
                Debug.LogWarning("既に生成済みなのにダンジョン生成メソッドを呼び出しているので無効");
                return;
            }
            _created = true;

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
            for (int i = 0; i < lines.Length; i++)
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
            if (TryGetComponent(out IDungeonBaseStringGenerator stringGenerator))
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

                    // ★:ここだけDungeonBuilderと違う
                    EntityType type = CharToEntityType(lines[i][k]);
                    MonoToEcsTransfer.Instance.AddData(tile.transform.position, Vector3.zero, type);

                    AddTileDataToDict(lines[i][k], tile);
                }
            }
        }

        EntityType CharToEntityType(char letter)
        {
            if (letter == '_' || letter == '@') return EntityType.Grass;
            else if (letter == '#') return EntityType.Wall;
            else if (letter == '<') return EntityType.SpawnPoint;
            else
            {
                throw new System.ArgumentException("タイルの文字がEntityに対応していない: " + letter);
            }
        }

        /// <summary>
        /// 外部から生成したダンジョンのタイルのデータを参照できるように辞書に追加する
        /// </summary>
        void AddTileDataToDict(char key, GameObject value)
        {
            if (!_tileDataDict.ContainsKey(key))
            {
                _tileDataDict.Add(key, new List<GameObject>());
            }

            _tileDataDict[key].Add(value);
        }
    }
}
