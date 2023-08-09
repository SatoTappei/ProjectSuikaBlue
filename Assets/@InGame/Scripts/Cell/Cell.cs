using UnityEngine;

namespace Cell
{
    public class Cell : MonoBehaviour, IHeightProvider
    {
        int _height;

        void Start()
        {

        }

        void Update()
        {

        }

        /// <summary>
        /// パーリンノイズで得られたセルの高さが少数点以下なので1000倍して整数に成形する
        /// </summary>
        void IHeightProvider.SetHeight(float height) => _height = (int)(height * 1000);

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            // 全セルに対して行うと非常に重いので普段はオフにしておくことを推奨
            CellGizmosDrawer.DrawHeight(transform.position, _height);
        }
#endif
    }
}
