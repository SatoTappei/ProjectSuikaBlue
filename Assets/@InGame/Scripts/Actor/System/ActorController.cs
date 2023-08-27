using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UniRx;

namespace PSB.InGame
{
    public class ActorController : MonoBehaviour
    {
        [SerializeField] int _spawnRadius = 5;

        Leader _leader;
        Actor _kinpatsuLeader;
        LinkedList<Actor> _kinpatsuList = new();
        LinkedList<Actor> _kurokamiList = new();

        Queue<Actor> _tempQueue = new();

        void Awake()
        {
            Actor.OnSpawned += AddActor;
        }

        void OnDestroy()
        {
            Actor.OnSpawned -= AddActor;
        }

        void Update()
        {
            AddFromTemp();

            // �����Ń^�[�����i�ރ^�[���x�[�X�ƍl�����Logic�������₷����������Ȃ�

            if (Input.GetKeyDown(KeyCode.Space)) TrySpawnKurokami();
            // �e�X�g:�L�[���͂ŏW��������
            if (Input.GetKey(KeyCode.LeftShift))
            {
                ForEachAll(actor => actor.Leader = _kinpatsuLeader.transform);
                float[] eval = _kinpatsuLeader.LeaderEvaluate();
                ForEachAll(actor => actor.Evaluate(eval));
            }
            else
            {
                ForEachAll(actor => actor.Evaluate()); // <- �e�X�g�Ŗ��t���[���]������
            }

            ForEachAll(actor => actor.StepParams());
            ForEachAll(actor => actor.StepAction());
        }

        /// <summary>
        /// ���[�_�[�̈ʒu�𒆐S�Ɉ��Ԋu���ꂽ�ʒu�ɐ�������
        /// </summary>
        /// <returns>��������:true �����ł��Ȃ�����:false</returns>
        bool TrySpawnKurokami()
        {
            if (_kinpatsuLeader == null) return false;
          
            Vector3 spawnPos = _kinpatsuLeader.transform.position;
            foreach (Vector2Int dir in Utility.EightDirections.OrderBy(_ => System.Guid.NewGuid()))
            {
                Vector3 pos = spawnPos + new Vector3(dir.x, 0, dir.y) * _spawnRadius;

                // �Z�����擾�o�����B�Z�����C�ȊO�A�����Ȃ��A�L���������Ȃ��ꍇ�͐����\
                if (!FieldManager.Instance.TryGetCell(pos, out Cell cell)) continue;
                if (!cell.IsEmpty) continue;

                MessageBroker.Default.Publish(new KurokamiSpawnMessage() { Pos = cell.Pos });
                return true;
            }

            return false;
        }

        void ForEachAll(UnityAction<Actor> action)
        {
            if (_kinpatsuLeader != null)
            {
                action.Invoke(_kinpatsuLeader);
            }

            foreach (Actor kinpatsu in _kinpatsuList)
            {
                action.Invoke(kinpatsu);
            }
            foreach (Actor kurokami in _kurokamiList)
            {
                action.Invoke(kurokami);
            }
        }

        void AddActor(Actor actor)
        {
            _tempQueue.Enqueue(actor);
            //if      (actor.Type == ActorType.KinpatsuLeader) _kinpatsuLeader = actor;
            //else if (actor.Type == ActorType.Kinpatsu)       _kinpatsuList.AddLast(actor);
            //else if (actor.Type == ActorType.Kurokami)       _kurokamiList.AddLast(actor);
            //else
            //{
            //    string msg = "�L�����N�^�[�̎�ނ�None�Ȃ̂�Controller�Ő���s�\: " + actor.name;
            //    throw new System.ArgumentException(msg);
            //}
        }

        void AddFromTemp()
        {
            while (_tempQueue.Count > 0)
            {
                Actor actor = _tempQueue.Dequeue();

                if (actor.Type == ActorType.KinpatsuLeader) _kinpatsuLeader = actor;
                else if (actor.Type == ActorType.Kinpatsu) _kinpatsuList.AddLast(actor);
                else if (actor.Type == ActorType.Kurokami) _kurokamiList.AddLast(actor);
                else
                {
                    string msg = "�L�����N�^�[�̎�ނ�None�Ȃ̂�Controller�Ő���s�\: " + actor.name;
                    throw new System.ArgumentException(msg);
                }
            }
        }
    }
}