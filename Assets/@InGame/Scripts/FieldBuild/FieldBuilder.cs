using UnityEngine;

namespace FieldBuild
{
    [RequireComponent(typeof(PerlinNoise))]
    [RequireComponent(typeof(Visualizer))]
    public class FieldBuilder : MonoBehaviour
    {
        [SerializeField] int _width = 50;
        [SerializeField] int _height = 50;

        void Start()
        {
            Build();
        }

        /// <summary>
        /// �m�C�Y�A�����A�Z���ɐݒ� �ƌv3�� ��*���� ��for�����񂵂Ă���̂Ŕ����
        /// </summary>
        void Build()
        {
            // �p�[�����m�C�Y
            PerlinNoise perlinNoise = GetComponent<PerlinNoise>();
            float[,] grid = perlinNoise.Create(_height, _width);
            // �Ή������I�u�W�F�N�g����
            Visualizer visualizer = GetComponent<Visualizer>();
            GameObject[,] field = visualizer.Instantiate(grid);
            // �Z���ɍ�����ݒ�
            SetCellHeight(grid, field);
        }

        void SetCellHeight(float[,] grid, GameObject[,] field)
        {
            for(int i = 0; i < field.GetLength(0); i++)
            {
                for(int k = 0; k < field.GetLength(1); k++)
                {
                    SetHeight(grid[i, k], field[i, k]);
                }
            }
        }

        void SetHeight(float height, GameObject cell)
        {
            if (!cell.TryGetComponent(out IHeightProvider holder))
            {
                string message = "�Z���̍�����ݒ肷��̂ɕK�v��IHeightProvider����������Ă��Ȃ�";
                throw new System.NullReferenceException(message);
            }
            holder.SetHeight(height);
        }
    }
}
