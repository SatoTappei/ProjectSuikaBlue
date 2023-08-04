using Unity.Entities;
using UnityEngine;

namespace MiniGameECS
{
    public class RandomValueAuthoring : MonoBehaviour
    {
        [SerializeField] uint _seed;

        class Baker : Baker<RandomValueAuthoring>
        {
            public override void Bake(RandomValueAuthoring authoring)
            {
                RandomData randomData = new()
                {
                    Seed = authoring._seed,
                    Random = Unity.Mathematics.Random.CreateFromIndex(authoring._seed),
                };
                AddComponent(GetEntity(TransformUsageFlags.None), randomData);
            }
        }
    }
}
