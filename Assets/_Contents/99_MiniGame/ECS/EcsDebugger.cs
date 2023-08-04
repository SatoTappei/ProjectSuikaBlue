using UnityEngine;
using UnityEngine.SceneManagement;

namespace MiniGameECS
{
    public class EcsDebugger : MonoBehaviour
    {
        Vector3[] _dirs =
        {
            Vector3.forward,
            Vector3.back,
            Vector3.right,
            Vector3.left,
        };

        void Update()
        {
            Unity.Mathematics.Random random = new();
            random.InitState((uint)UnityEngine.Random.Range(0, 10000));
            if (Input.GetKeyDown(KeyCode.Space))
            {
                for (int i = 0; i < 1; i++)
                {
                    Vector2 r = random.NextFloat2Direction();
                    Vector3 dir = _dirs[UnityEngine.Random.Range(0, _dirs.Length)];
                    MonoToEcsTransfer.Instance.AddData(Vector3.zero, dir, EntityType.Debris);
                }
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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