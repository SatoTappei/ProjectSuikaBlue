using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

namespace BoidECS
{
    //[WriteGroup(typeof(LocalTransform))] <- 謎の属性、とりあえずコメントアウトしておく
    public struct BirdData : ISharedComponentData
    {
        public float NeighbourRadius;
        public float SeparationWeight;
        public float AlignmentWeight;
        public float CohesionWeight;
        public float AvoidanceDistance;
        public float Speed;
    }

    public class BirdAuthoring : MonoBehaviour
    {
        public float _neighbourRadius = 8.0f;
        public float _separationWeight = 1.0f;
        public float _alignmentWeight = 1.0f;
        public float _cohesionWeight = 2.0f;
        public float _avoidanceDistance = 30.0f;
        public float _speed = 25.0f;

        class Baker : Baker<BirdAuthoring>
        {
            public override void Bake(BirdAuthoring authoring)
            {
                AddSharedComponent(GetEntity(TransformUsageFlags.Dynamic), new BirdData()
                {
                    NeighbourRadius = authoring._neighbourRadius,
                    SeparationWeight = authoring._separationWeight,
                    AlignmentWeight = authoring._alignmentWeight,
                    CohesionWeight = authoring._cohesionWeight,
                    AvoidanceDistance = authoring._avoidanceDistance,
                    Speed = authoring._speed,
                });
            }
        }
    }
}
