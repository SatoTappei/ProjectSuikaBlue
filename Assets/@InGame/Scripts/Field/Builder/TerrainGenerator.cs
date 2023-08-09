using UnityEngine;

namespace Field
{
    public class TerrainGenerator : MonoBehaviour
    {
        [SerializeField] GameObject _grass;
        [SerializeField] GameObject _soil;
        [SerializeField] GameObject _water;
        [SerializeField] float _waterBorder = 0.35f;
        [SerializeField] float _soilBorder = 0.43f;
        [Header("�^�C���Ɋ��蓖�Ă�}�e���A��")]
        [SerializeField] Material _grassMat1;
        [SerializeField] Material _grassMat2;
        [SerializeField] Material _soilMat1;
        [SerializeField] Material _soilMat2;
        [SerializeField] Material _waterMat1;
        [SerializeField] Material _waterMat2;

        /// <summary>
        /// ���������p�[�����m�C�Y�̃O���b�h�ɑΉ������I�u�W�F�N�g�𐶐�����
        /// </summary>
        /// <returns>�����ƃ^�C���̎�ނ��������񂾃Z���̓񎟌��z��</returns>
        public Cell[,] Create(float[,] grid)
        {
            Cell[,] field = new Cell[grid.GetLength(0), grid.GetLength(1)];
            // �e
            Transform parent = new GameObject("Field").transform;
            GameObject grassParent1 = CreateTileParent("GrassParent_1", parent);
            GameObject grassParent2 = CreateTileParent("GrassParent_2", parent);
            GameObject soilParent1 = CreateTileParent("SoilParent_1", parent);
            GameObject soilParent2 = CreateTileParent("SoilParent_2", parent);
            GameObject waterParent1 = CreateTileParent("WaterParent_1", parent);
            GameObject waterParent2 = CreateTileParent("WaterParent_2", parent);

            float h = (grid.GetLength(0) / 2) + (grid.GetLength(0) / 2 % 2 == 0 ? 0.5f : 0);
            float w = (grid.GetLength(1) / 2) + (grid.GetLength(1) / 2 % 2 == 0 ? 0.5f : 0);
            for (int i = 0; i < grid.GetLength(0); i++)
            {
                for (int k = 0; k < grid.GetLength(1); k++)
                {
                    // ��������^�C��������
                    GameObject prefab;
                    GameObject tileParent;
                    TileType type = HightToCellType(grid[i, k]);
                    bool choice = Random.value <= 0.5f; // �^�C������2�̃o���G�[�V�������猈�߂�t���O

                    if      (type == TileType.Water && choice)  { prefab = _water; tileParent = waterParent1; }
                    else if (type == TileType.Water && !choice) { prefab = _water; tileParent = waterParent2; }
                    else if (type == TileType.Soil  && choice)  { prefab = _soil;  tileParent = soilParent1; }
                    else if (type == TileType.Soil  && !choice) { prefab = _soil;  tileParent = soilParent2; }
                    else if (type == TileType.Grass && choice)  { prefab = _grass; tileParent = grassParent1; }
                    else if (type == TileType.Grass && !choice) { prefab = _grass; tileParent = grassParent2; }
                    else throw new System.Exception($"�^�C���̏��������v���Ȃ�: {type} {choice}");
                    
                    // �^�C���̐���
                    Vector3 pos = new Vector3(k - h, 0, i - w);
                    GameObject tile = Instantiate(prefab, pos, Quaternion.identity);
                    tile.transform.SetParent(tileParent.transform);
                    // �Z���̐���
                    field[i, k] = new(type, pos, grid[i, k]);
                }
            }

            MeshCombine(grassParent1, _grassMat1);
            MeshCombine(grassParent2, _grassMat2);
            MeshCombine(soilParent1, _soilMat1);
            MeshCombine(soilParent2, _soilMat2);
            MeshCombine(waterParent1, _waterMat1);
            MeshCombine(waterParent2, _waterMat2);

            return field;
        }

        /// <summary>
        /// ��ނ��Ƃ̃^�C���̐e�𐶐����A���b�V���̌����ɕK�v�Ȋe�R���|�[�l���g���A�^�b�`����B
        /// ���������^�C���̐e�̓q�G�����L�[�̐����̂��߁A��2������e�Ƃ��ēo�^����
        /// </summary>
        /// <returns>��ނ��Ƃ̃^�C���̐e</returns>
        GameObject CreateTileParent(string name, Transform parent)
        {
            GameObject tileParent = new GameObject(name);
            tileParent.AddComponent<MeshFilter>();
            tileParent.AddComponent<MeshRenderer>();
            tileParent.transform.SetParent(parent);

            return tileParent;
        }

        TileType HightToCellType(float height)
        {
            if      (height < _waterBorder) return TileType.Water;
            else if (height < _soilBorder)  return TileType.Soil;
            else                            return TileType.Grass;
        }

        /// <summary>
        /// ���b�V������������
        /// </summary>
        void MeshCombine(GameObject tileParent, Material material)
        {
            MeshFilter[] filters = tileParent.GetComponentsInChildren<MeshFilter>();
            // ���[�v�̃J�E���^�� 1 �n�܂�ɂ��Đe�I�u�W�F�N�g�̃��b�V��������
            CombineInstance[] combine = new CombineInstance[filters.Length - 1];
            for(int i = 1; i < filters.Length; i++)
            {
                combine[i - 1].mesh = filters[i].sharedMesh;
                combine[i - 1].transform = filters[i].transform.localToWorldMatrix;
                // ���̃��b�V���͔�\���ɂ��Ă���
                filters[i].gameObject.SetActive(false);
            }

            MeshFilter filter = tileParent.GetComponent<MeshFilter>();
            filter.mesh = new Mesh();
            filter.mesh.CombineMeshes(combine);

            MeshRenderer renderer = tileParent.GetComponent<MeshRenderer>();
            renderer.material = material;
        }
    }
}
