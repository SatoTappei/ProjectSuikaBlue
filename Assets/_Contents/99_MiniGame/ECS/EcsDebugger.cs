using UnityEngine;

namespace MiniGameECS
{
    public class EcsDebugger : MonoBehaviour
    {
        [SerializeField] GameObject _test;
        void Start()
        {
            //Unity.Mathematics.Random random = new();
            //random.InitState((uint)UnityEngine.Random.Range(0, 10000));
            //for (int i = 0; i < 1000; i++)
            //{
            //    Vector3 rPos = random.NextFloat3Direction() * random.NextFloat(6.0f);
            //    Vector3 rDir = random.NextFloat3Direction() * random.NextFloat(6.0f);
            //    GameObject go = Instantiate(_test);
            //    go.transform.position = rPos;
            //    //MonoToEcsTransfer.Instance.AddData(rPos, rDir, EntityType.Debris);
            //}
        }

        void Update()
        {


            Unity.Mathematics.Random random = new();
            random.InitState((uint)UnityEngine.Random.Range(0, 10000));
            if (Input.GetKeyDown(KeyCode.Space))
            {
                for (int i = 0; i < 1000; i++)
                {
                    Vector3 rPos = random.NextFloat3Direction() * random.NextFloat(6.0f);
                    Vector3 rDir = random.NextFloat3Direction() * random.NextFloat(6.0f);
                    MonoToEcsTransfer.Instance.AddData(rPos, rDir, EntityType.Debris);
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