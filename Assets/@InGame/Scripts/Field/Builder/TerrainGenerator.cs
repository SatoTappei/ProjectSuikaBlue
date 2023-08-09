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
        [Header("タイルに割り当てるマテリアル")]
        [SerializeField] Material _grassMat1;
        [SerializeField] Material _grassMat2;
        [SerializeField] Material _soilMat1;
        [SerializeField] Material _soilMat2;
        [SerializeField] Material _waterMat1;
        [SerializeField] Material _waterMat2;

        /// <summary>
        /// 生成したパーリンノイズのグリッドに対応したオブジェクトを生成する
        /// </summary>
        /// <returns>高さとタイルの種類を書き込んだセルの二次元配列</returns>
        public Cell[,] Create(float[,] grid)
        {
            Cell[,] field = new Cell[grid.GetLength(0), grid.GetLength(1)];
            // 親
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
                    // 生成するタイルを決定
                    GameObject prefab;
                    GameObject tileParent;
                    TileType type = HightToCellType(grid[i, k]);
                    bool choice = Random.value <= 0.5f; // タイル毎の2つのバリエーションから決めるフラグ

                    if      (type == TileType.Water && choice)  { prefab = _water; tileParent = waterParent1; }
                    else if (type == TileType.Water && !choice) { prefab = _water; tileParent = waterParent2; }
                    else if (type == TileType.Soil  && choice)  { prefab = _soil;  tileParent = soilParent1; }
                    else if (type == TileType.Soil  && !choice) { prefab = _soil;  tileParent = soilParent2; }
                    else if (type == TileType.Grass && choice)  { prefab = _grass; tileParent = grassParent1; }
                    else if (type == TileType.Grass && !choice) { prefab = _grass; tileParent = grassParent2; }
                    else throw new System.Exception($"タイルの条件が合致しない: {type} {choice}");
                    
                    // タイルの生成
                    Vector3 pos = new Vector3(k - h, 0, i - w);
                    GameObject tile = Instantiate(prefab, pos, Quaternion.identity);
                    tile.transform.SetParent(tileParent.transform);
                    // セルの生成
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
        /// 種類ごとのタイルの親を生成し、メッシュの結合に必要な各コンポーネントをアタッチする。
        /// 生成したタイルの親はヒエラルキーの整理のため、第2引数を親として登録する
        /// </summary>
        /// <returns>種類ごとのタイルの親</returns>
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
        /// メッシュを結合する
        /// </summary>
        void MeshCombine(GameObject tileParent, Material material)
        {
            MeshFilter[] filters = tileParent.GetComponentsInChildren<MeshFilter>();
            // ループのカウンタを 1 始まりにして親オブジェクトのメッシュを除く
            CombineInstance[] combine = new CombineInstance[filters.Length - 1];
            for(int i = 1; i < filters.Length; i++)
            {
                combine[i - 1].mesh = filters[i].sharedMesh;
                combine[i - 1].transform = filters[i].transform.localToWorldMatrix;
                // 元のメッシュは非表示にしておく
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
