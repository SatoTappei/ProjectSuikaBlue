using UnityEngine;

/// <summary>
/// 経路テーブルを用いた経路探索用の頂点
/// PathTableクラスが保持している頂点のオブジェクトに対して実行時にAddComponentされる
/// </summary>
public class PathVertex : MonoBehaviour
{
    public int Number { get; set; }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            // 頂点番号の表示
            Vector3 pos = transform.position + Vector3.up * 1.5f;
            string str = $"頂点: {Number}";
            UnityEditor.Handles.color = Color.black;
            UnityEditor.Handles.Label(pos, str);
        }
#endif
    }
}
