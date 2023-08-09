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
            // �p�[�����m�C�Y�œ���ꂽ�Z���̍����������_�ȉ��Ȃ̂�1000�{���Đ����ɐ��`����
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
        /// Addressable�ɓo�^�����A�h���X��Ԃ�
        /// </summary>
        /// <returns>Addressable�ɓo�^�����A�h���X</returns>
        string ResourceTypeToAddress(ResourceType type)
        {
            if      (type == ResourceType.PalmTree) return "PalmTree";
            else if (type == ResourceType.Tree)     return "Tree";
            else if (type == ResourceType.Stone)    return "Stone";
            else return string.Empty;
        }

        public void Release()
        {
            Debug.Log("���");
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
        //            // �S�Z���ɑ΂��čs���Ɣ��ɏd���̂ŕ��i�̓I�t�ɂ��Ă������Ƃ𐄏�
        //            CellGizmosDrawer.DrawHeight(transform.position, _height);
        //        }
        //#endif
    }
}

