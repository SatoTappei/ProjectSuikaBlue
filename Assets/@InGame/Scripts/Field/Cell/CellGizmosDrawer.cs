using UnityEngine;

namespace PSB.InGame
{
#if UNITY_EDITOR
    public class CellGizmosDrawer
    {
        // ƒVƒ“ƒOƒ‹ƒgƒ“
        static CellGizmosDrawer Instance = new();

        GUIStyle _style;
        GUIStyleState _styleState;

        CellGizmosDrawer()
        {
            _styleState = new GUIStyleState();
            _style = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                normal = _styleState,
            };
        }

        public static void DrawHeight(Vector3 pos, int height)
        {
            UnityEditor.Handles.Label(pos, height.ToString(), Instance._style);
        }
    }
#endif
}