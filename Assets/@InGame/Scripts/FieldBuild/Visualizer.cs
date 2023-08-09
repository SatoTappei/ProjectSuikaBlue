using System.Collections.Generic;
using UnityEngine;

namespace FieldBuild
{
    public class Visualizer : MonoBehaviour
    {
        [SerializeField] Material _grassMat1;
        [SerializeField] Material _grassMat2;
        [SerializeField] Material _soilMat1;
        [SerializeField] Material _soilMat2;
        [SerializeField] Material _waterMat1;
        [SerializeField] Material _waterMat2;

        [SerializeField] GameObject _grass;
        [SerializeField] GameObject _soil;
        [SerializeField] GameObject _water;
        [SerializeField] float _waterBorder = 0.35f;
        [SerializeField] float _soilBorder = 0.43f;

        /// <summary>
        /// 生成したパーリンノイズのグリッドに対応したオブジェクトを生成する
        /// </summary>
        /// /// <returns>パーリンノイズから生成したフィールド</returns>
        public GameObject[,] Instantiate(float[,] grid)
        {
            GameObject[,] field = new GameObject[grid.GetLength(0), grid.GetLength(1)];
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
                    GameObject prefab;
                    Transform tileParent;
                    if (grid[i, k] < _waterBorder) 
                    { 
                        prefab = _water; 
                        tileParent = Random.value <= 0.5f ? waterParent1.transform : waterParent2.transform;
                    }
                    else if (grid[i, k] < _soilBorder)
                    { 
                        prefab = _soil;  
                        tileParent = Random.value <= 0.5f ? soilParent1.transform : soilParent2.transform;
                    }
                    else                                
                    { 
                        prefab = _grass;
                        tileParent = Random.value <= 0.5f ? grassParent1.transform : grassParent2.transform;
                    }

                    field[i, k] = Instantiate(prefab, new Vector3(k - h, 0, i - w), Quaternion.identity);
                    field[i, k].transform.SetParent(tileParent);
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

        void MeshCombine(GameObject tileParent, Material material)
        {
            MeshFilter[] meshFilters = tileParent.GetComponentsInChildren<MeshFilter>();
            List<MeshFilter> meshFilterList = new List<MeshFilter>();
            for (int i = 1; i < meshFilters.Length; i++)
            {
                meshFilterList.Add(meshFilters[i]);
            }
            CombineInstance[] combine = new CombineInstance[meshFilterList.Count];
            // 結合するメッシュの情報をCombineInstanceに追加していきます。
            for (int i = 0; i < meshFilterList.Count; i++)
            {
                combine[i].mesh = meshFilterList[i].sharedMesh;
                combine[i].transform = meshFilterList[i].transform.localToWorldMatrix;
                meshFilterList[i].gameObject.SetActive(false);
            }

            MeshFilter filter = tileParent.GetComponent<MeshFilter>();
            MeshRenderer renderer = tileParent.GetComponent<MeshRenderer>();

            // 結合したメッシュをセットします。
            filter.mesh = new Mesh();
            filter.mesh.CombineMeshes(combine);

            // 結合したメッシュにマテリアルをセットします。
            renderer.material = material;

            // 親オブジェクトを表示します。
            tileParent.gameObject.SetActive(true);
        }
    }
}
