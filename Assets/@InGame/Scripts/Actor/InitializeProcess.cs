using UnityEngine;

namespace PSB.InGame
{
    /// <summary>
    /// キャラクターの初期化処理のみを分離したクラス
    /// 遺伝子からステータスを作成し、サイズと色を反映させる
    /// </summary>
    public class InitializeProcess : MonoBehaviour
    {
        [Header("サイズの反映")]
        [SerializeField] Transform _model;
        [Header("色の反映")]
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
            // 色変更後のmaterialはコピーされるのでGameObjectの破棄とともに削除する
            if (_copyMaterial) Destroy(_copyMaterial);
        }
    }
}