using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// �L�����N�^�[�̏����������݂̂𕪗������N���X
    /// ��`�q����X�e�[�^�X���쐬���A�T�C�Y�ƐF�𔽉f������
    /// </summary>
    public class InitializeProcess : MonoBehaviour
    {
        [Header("�T�C�Y�̔��f")]
        [SerializeField] Transform _model;
        [Header("�F�̔��f")]
        [SerializeField] SkinnedMeshRenderer _renderer;

        Material _copyMaterial;

        public Status Execute(uint? gene, ActorType type)
        {
            Status status = ApplyStatus(gene, type);
            ApplyInheritedSize(status.Size);
            ApplyInheritedColor(status.Color);

            return status;
        }

        Status ApplyStatus(uint? gene, ActorType type)
        {
            StatusBase statusBase = StatusBaseHolder.Get(type);
            gene ??= statusBase.DefaultGene;

            return new(statusBase, (uint)gene);
        }

        void ApplyInheritedSize(float size)
        {
            _model.transform.localScale *= size;
        }

        void ApplyInheritedColor(Color32 color)
        {
            _renderer.material.SetColor("_BaseColor", color);
            _copyMaterial = _renderer.material;
        }

        void OnDestroy()
        {
            // �F�ύX���material�̓R�s�[�����̂�GameObject�̔j���ƂƂ��ɍ폜����
            if (_copyMaterial) Destroy(_copyMaterial);
        }
    }
}