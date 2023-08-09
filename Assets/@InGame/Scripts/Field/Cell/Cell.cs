using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Field
{
    public enum TileType
    {
        Water,
        Soil,
        Grass,
    }

    public enum ResourceType
    {
        None,
        PalmTree,
        Tree,
        Stone,
    }

    public class Cell
    {
        public Cell(TileType type, Vector3 pos, float height)
        {
            TileType = type;
            Pos = pos;
            // パーリンノイズで得られたセルの高さが少数点以下なので1000倍して整数に成形する
            Height = (int)(height * 1000);
        }

        GameObject _resource;
        
        public ResourceType ResourceType { get; private set; }
        public TileType TileType { get; }
        public Vector3 Pos { get; }
        public int Height { get; }

        public void CreateResource(ResourceType type)
        {
            ResourceType = type;
            if (type != ResourceType.None) Instantiate(type);
        }

        async void Instantiate(ResourceType type)
        {
            string address = ResourceTypeToAddress(type);
            _resource = await Addressables.InstantiateAsync(address).Task;
            _resource.transform.position = Pos;
        }

        /// <summary>
        /// Addressableに登録したアドレスを返す
        /// </summary>
        /// <returns>Addressableに登録したアドレス</returns>
        string ResourceTypeToAddress(ResourceType type)
        {
            if      (type == ResourceType.PalmTree) return "PalmTree";
            else if (type == ResourceType.Tree)     return "Tree";
            else if (type == ResourceType.Stone)    return "Stone";
            else return string.Empty;
        }

        public void Release()
        {
            Debug.Log("解放");
            if (_resource != null)
            {
                Addressables.ReleaseInstance(_resource);
            }
            
        }

        //        /// <summary>
        //        /// 
        //        /// </summary>
        //        void IHeightProvider.SetHeight(float height) => _height = (int)(height * 1000);

        //#if UNITY_EDITOR
        //        void OnDrawGizmos()
        //        {
        //            // 全セルに対して行うと非常に重いので普段はオフにしておくことを推奨
        //            CellGizmosDrawer.DrawHeight(transform.position, _height);
        //        }
        //#endif
    }
}

