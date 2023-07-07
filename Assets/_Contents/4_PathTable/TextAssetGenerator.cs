using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PathTableGraph
{
    /// <summary>
    /// 経路探索で求めた経路をテキスト形式で保存するクラス
    /// </summary>
    public class TextAssetGenerator
    {
        /// <summary>
        /// 出力先のパスは、ビルド後で変わるので注意
        /// </summary>
        readonly string FilePath = Path.Combine(Application.dataPath, "PathData.txt");

        public TextAssetGenerator()
        {
            // 1つのテキストファイルに複数回書き込むので
            // 予め描かれている内容は全削除する
            using (StreamWriter writer = new StreamWriter(FilePath, false)){ }
        }

        /// <summary>
        /// 1つのテキストファイルに、このメソッドを呼び出すたびに経路を書き込む
        /// </summary>
        public void WritePath(IEnumerable<Vector3> path, int startNumber, int goalNumber)
        {
            using (StreamWriter writer = new StreamWriter(FilePath, true))
            {
                writer.WriteLine($"{startNumber} {goalNumber}");
                foreach (Vector3 vertexPos in path)
                {
                    string str = $"{vertexPos.x} {vertexPos.y} {vertexPos.z}";
                    writer.WriteLine(str);
                }
            }
        }
    }
}
