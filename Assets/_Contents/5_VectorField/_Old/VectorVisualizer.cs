using UnityEngine;

namespace Old
{
    /// <summary>
    /// 8方向のベクトルを矢印で描画するクラス
    /// デバッグ用なので無くても動作する
    /// </summary>
    public class VectorVisualizer : MonoBehaviour
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
        /// 子オブジェクト(矢印)を全削除する
        /// ベクターフィールドの更新時に呼び出される
        /// </summary>
        public void RemoveAll()
        {
            while (_parent.transform.childCount > 0)
            {
                Destroy(_parent.transform.GetChild(0));
            }
        }

        public void VisualizeCellVector(Cell cell)
        {
            EightDirection dir = Vector3To8Direction(cell.Vector);
            if(dir != EightDirection.Neutral)
            {
                Add(cell.Pos, dir);
            }
        }

        /// <summary>
        /// Vector3に対応した8方向の列挙型を返す
        /// </summary>
        EightDirection Vector3To8Direction(Vector3 vector)
        {
            if (vector == new Vector3(0, 0, 1)) return EightDirection.North;
            if (vector == new Vector3(0, 0, -1)) return EightDirection.South;
            if (vector == new Vector3(1, 0, 0)) return EightDirection.East;
            if (vector == new Vector3(-1, 0, 0)) return EightDirection.West;
            if (vector == new Vector3(1, 0, 1)) return EightDirection.NorthEast;
            if (vector == new Vector3(1, 0, -1)) return EightDirection.SouthEast;
            if (vector == new Vector3(-1, 0, 1)) return EightDirection.NorthWest;
            if (vector == new Vector3(-1, 0, -1)) return EightDirection.SouthWest;

            Debug.LogError("ベクトルに対応した方向が無い: " + vector);
            return EightDirection.Neutral;
        }

        /// <summary>
        /// 指定した位置/方向の矢印を生成する
        /// </summary>
        void Add(Vector3 pos, EightDirection dir)
        {
            GameObject cursor = new GameObject("Cursor " + dir);
            cursor.transform.position = pos + Vector3.up * 0.01f;
            cursor.transform.SetParent(_parent.transform);
            SpriteRenderer sr = cursor.AddComponent<SpriteRenderer>();
            sr.sprite = _cursorSprite;
            
            // 8方向に対応した角度に回転させる
            switch (dir)
            {
                case EightDirection.North: cursor.transform.Rotate(90, -90, 0);
                    break;
                case EightDirection.South: cursor.transform.Rotate(90, 90, 0);
                    break;
                case EightDirection.West: cursor.transform.Rotate(90, 180, 0);
                    break;
                case EightDirection.East: cursor.transform.Rotate(90, 0, 0);
                    break;
                case EightDirection.NorthEast: cursor.transform.Rotate(90, -45, 0);
                    break;
                case EightDirection.NorthWest: cursor.transform.Rotate(90, 315, 0);
                    break;
                case EightDirection.SouthEast: cursor.transform.Rotate(90, 45, 0);
                    break;
                case EightDirection.SouthWest: cursor.transform.Rotate(90, 135, 0);
                    break;
            }
        }
    }
}

// 指定した位置のベクターフィールド作成 <- 完了
// 指定した位置からターゲットセルまで ＆ 指定した位置からグリッドの端まで
// Vector3型のコレクションで返す