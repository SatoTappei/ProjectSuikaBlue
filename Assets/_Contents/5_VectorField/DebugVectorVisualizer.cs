using UnityEngine;

namespace VectorField
{
    /// <summary>
    /// ベクトルフィールドを生成後、ベクトルの流れを8方向のベクトルを
    /// 矢印で描画するデバッグ用クラス
    /// </summary>
    [RequireComponent(typeof(VectorFieldManager))]
    public class DebugVectorVisualizer : MonoBehaviour
    {
        [SerializeField] Sprite _cursorSprite;
        GameObject _parent;

        void Awake()
        {
            _parent = new GameObject("VectorVisualizeCursors");
            _parent.transform.SetParent(transform);
        }

        public void Valid() => _parent.gameObject.SetActive(true);
        public void Invalid() => _parent.gameObject.SetActive(false);

        /// <summary>
        /// 矢印を全削除する
        /// ベクターフィールドの更新時に呼び出される
        /// </summary>
        public void RemoveAll()
        {
            while (_parent.transform.childCount > 0)
            {
                Destroy(_parent.transform.GetChild(0));
            }
        }

        /// <summary>
        /// セルのベクトルに対応した矢印を 2DSprite として生成する
        /// 斜め方向のベクトルは正規化済みである必要がある
        /// </summary>
        public void VisualizeCellVector(Cell cell)
        {
            if (cell.Vector == Vector3.zero) return;

            if      (cell.Vector == new Vector3(0, 0, 1)) CreateCursor(cell.Pos, 90, -90);
            else if (cell.Vector == new Vector3(0, 0, -1)) CreateCursor(cell.Pos, 90, 90);
            else if (cell.Vector == new Vector3(1, 0, 0)) CreateCursor(cell.Pos, 90, 0);
            else if (cell.Vector == new Vector3(-1, 0, 0)) CreateCursor(cell.Pos, 90, 180);
            else if (cell.Vector == new Vector3(1, 0, 1).normalized) CreateCursor(cell.Pos, 90, -45);
            else if (cell.Vector == new Vector3(1, 0, -1).normalized) CreateCursor(cell.Pos, 90, 45);
            else if (cell.Vector == new Vector3(-1, 0, 1).normalized) CreateCursor(cell.Pos, 90, 315);
            else if (cell.Vector == new Vector3(-1, 0, -1).normalized) CreateCursor(cell.Pos, 90, 135);
            else
            {
                throw new System.ArgumentException("ベクトルの値が8方向の矢印に対応していない: " + cell.Vector);
            }
        }

        /// <summary>
        /// 指定した位置/方向の矢印を生成する
        /// </summary>
        void CreateCursor(Vector3 pos, float rotX, float rotY)
        {
            GameObject cursor = new GameObject("Cursor");
            cursor.transform.position = pos + Vector3.up * 5.0f;
            cursor.transform.SetParent(_parent.transform);
            cursor.transform.Rotate(rotX, rotY, 0);
            SpriteRenderer sr = cursor.AddComponent<SpriteRenderer>();
            sr.sprite = _cursorSprite;
        }
    }
}