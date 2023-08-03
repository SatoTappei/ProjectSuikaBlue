using UnityEngine;

namespace MiniGameECS
{
    public class EcsDebugger : MonoBehaviour
    {
        void Update()
        {
            Unity.Mathematics.Random random = new();
            random.InitState((uint)UnityEngine.Random.Range(0, 10000));
            if (Input.GetKeyDown(KeyCode.Space))
            {
                for (int i = 0; i < 1; i++)
                {
                    Vector2 r = random.NextFloat2Direction();
                    Vector3 dir = new Vector3(r.x, 0, r.y);
                    MonoToEcsTransfer.Instance.AddData(Vector3.zero, dir, EntityType.Debris);
                }
            }
        }

        void M()
        {
            //// エンティティマネージャー取得
            //EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            //// Entity作成
            //Entity entity = manager.CreateEntity();
            //// バッファーの作成＆値の追加
            //DynamicBuffer<BufferData> buffer = manager.AddBuffer<BufferData>(entity);
            //buffer.Add(new BufferData { Value = 1 });
            //buffer.Add(new BufferData { Value = 2 });
            //buffer.Add(new BufferData { Value = 3 });
            //// ???:bufferを元にint型のバッファーを新しく作成？
            //DynamicBuffer<int> intBuffer = buffer.Reinterpret<int>();
            //intBuffer[1] = 5;

            //// ↓これでも値の変更はできる
            ////buffer[1] = new BufferData { Value = 55 };
        }
    }
}