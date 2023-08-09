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
        /// �p�[�����m�C�Y�œ���ꂽ�Z���̍����������_�ȉ��Ȃ̂�1000�{���Đ����ɐ��`����
        /// </summary>
        void IHeightProvider.SetHeight(float height) => _height = (int)(height * 1000);

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            // �S�Z���ɑ΂��čs���Ɣ��ɏd���̂ŕ��i�̓I�t�ɂ��Ă������Ƃ𐄏�
            CellGizmosDrawer.DrawHeight(transform.position, _height);
        }
#endif
    }
}
