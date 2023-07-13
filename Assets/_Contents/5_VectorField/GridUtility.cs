using UnityEngine;

namespace VectorField
{
    /// <summary>
    /// �x�N�g���t�B�[���h�̃O���b�h�Ŏg�p���鋤�ʏ����𔲂��o�����N���X
    /// </summary>
    public static class GridUtility
    {
        /// <summary>
        /// ���[���h���W�ɑΉ������O���b�h�̓Y������Ԃ�
        /// </summary>
        public static Vector2Int WorldPosToGridIndex(Vector3 targetPos, Cell[,] grid, GridData data)
        {
            // �O���b�h��1�ӂ̒���
            float forwardZ = grid[0, 0].Pos.z;
            float backZ = grid[data.Height - 1, data.Width - 1].Pos.z;
            float leftX = grid[0, 0].Pos.x;
            float rightX = grid[data.Height - 1, data.Width - 1].Pos.x;
            // �O���b�h�̒[������W�܂ł̒���
            float lengthZ = backZ - forwardZ;
            float lengthX = rightX - leftX;
            // �O���b�h�̒[���牽���̈ʒu��
            float fromPosZ = targetPos.z - forwardZ;
            float fromPosX = targetPos.x - leftX;
            // �O���b�h�̒[���牽���̈ʒu��
            float percentZ = Mathf.Abs(fromPosZ / lengthZ);
            float percentX = Mathf.Abs(fromPosX / lengthX);

            // x�͂��̂܂܁Ay��z�ɑΉ����Ă���
            Vector2Int index = new Vector2Int()
            {
                x = Mathf.RoundToInt((data.Width - 1) * percentX),
                y = Mathf.RoundToInt((data.Height - 1) * percentZ),
            };

            return index;
        }
    }
}
