using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using System;

namespace MyECS
{
    public struct ActorData : IComponentData
    {
        public float RotSpeed;
        public float MoveSpeed;
        public float MoveRadius;

        /// <summary>
        /// シード値から各値がランダムなActorDataを作成して返す
        /// </summary>
        public static ActorData Random(uint seed)
        {
            return new ActorData()
            {
                RotSpeed = new Unity.Mathematics.Random(seed).NextFloat(1.0f, 6.0f),
                MoveSpeed = new Unity.Mathematics.Random(seed).NextFloat(1.0f, 6.0f),
                MoveRadius = new Unity.Mathematics.Random(seed).NextFloat(3.0f, 6.0f),
            };
        }
    }

    public class ActorAuthoring : MonoBehaviour
    {
        public float _rotSpeed = 3.0f;
        public float _moveSpeed = 3.0f;
        public float _moveRadius = 10.0f;

        class Baker : Baker<ActorAuthoring>
        {
            public override void Bake(ActorAuthoring authoring)
            {
                ActorData actorData = new()
                {
                    RotSpeed = authoring._rotSpeed,
                    MoveSpeed = authoring._moveSpeed,
                    MoveRadius = authoring._moveRadius,
                };
                AddComponent(GetEntity(TransformUsageFlags.Dynamic), actorData);
            }
        }
    }
}
