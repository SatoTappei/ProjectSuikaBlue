using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace PathTableGraph
{
    /// <summary>
    /// �o�H�T���ŋ��߂��o�H���e�L�X�g�`���ŕۑ�����N���X
    /// </summary>
    public class TextAssetGenerator
    {
        /// <summary>
        /// �o�͐�̃p�X�́A�r���h��ŕς��̂Œ���
        /// </summary>
        readonly string FilePath = Path.Combine(Application.dataPath, "PathData.txt");

        public TextAssetGenerator()
        {
            // 1�̃e�L�X�g�t�@�C���ɕ����񏑂����ނ̂�
            // �\�ߕ`����Ă�����e�͑S�폜����
            using (StreamWriter writer = new StreamWriter(FilePath, false)){ }
        }

        /// <summary>
        /// 1�̃e�L�X�g�t�@�C���ɁA���̃��\�b�h���Ăяo�����тɌo�H����������
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
