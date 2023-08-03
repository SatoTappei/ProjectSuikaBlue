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
            //// �G���e�B�e�B�}�l�[�W���[�擾
            //EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
            //// Entity�쐬
            //Entity entity = manager.CreateEntity();
            //// �o�b�t�@�[�̍쐬���l�̒ǉ�
            //DynamicBuffer<BufferData> buffer = manager.AddBuffer<BufferData>(entity);
            //buffer.Add(new BufferData { Value = 1 });
            //buffer.Add(new BufferData { Value = 2 });
            //buffer.Add(new BufferData { Value = 3 });
            //// ???:buffer������int�^�̃o�b�t�@�[��V�����쐬�H
            //DynamicBuffer<int> intBuffer = buffer.Reinterpret<int>();
            //intBuffer[1] = 5;

            //// ������ł��l�̕ύX�͂ł���
            ////buffer[1] = new BufferData { Value = 55 };
        }
    }
}