using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UniRx;
using UniRx.Triggers;

namespace PSB.InGame
{
    /// <summary>
    /// �Q�[���S�̂̃��W�b�N�̐�����s���B
    /// �t�B�[���h����уL�����N�^�[�̐����A�e�L�����N�^�[�̍X�V�������s���B
    /// </summary>
    public class GameLogic : MonoBehaviour
    {
        [SerializeField] InitKinpatsuSpawner _initKinpatsuSpawner;
        [SerializeField] int _spawnRadius = 5;

        Leader _leader;
        Actor _kinpatsuLeader;
        LinkedList<Actor> _kinpatsuList = new();
        LinkedList<Actor> _kurokamiList = new();

        bool _initialized;
        // �C�ӂ̃^�C�~���O�ŃL�����N�^�[�̃��X�g���X�V���邽�߂̈ꎞ�ۑ��p�̃L���[
        Queue<Actor> _temp = new();

        async void Start()
        {
            // �L�����N�^�[�𐶐������ۂɂ��̃N���X�Ő���ł���悤�ɓo�^����
            RegisterActorCallback();

            _initialized = await InitAsync(this.GetCancellationTokenOnDestroy());
        }

        void Update()
        {
            if (!_initialized) return;

            // ���䂷��L�����N�^�[�̃��X�g�̍X�V
            AddControledActorFromTemp();

            //// �����Ń^�[�����i�ރ^�[���x�[�X�ƍl�����Logic�������₷����������Ȃ�

            //if (Input.GetKeyDown(KeyCode.Space)) TrySpawnKurokami();
            //// �e�X�g:�L�[���͂ŏW��������
            //if (Input.GetKey(KeyCode.LeftShift))
            //{
            //    ForEachAll(actor => actor.Leader = _kinpatsuLeader.transform);
            //    float[] eval = _kinpatsuLeader.LeaderEvaluate();
            //    ForEachAll(actor => actor.Evaluate(eval));
            //}
            //else
            //{
            //    ForEachAll(actor => actor.Evaluate()); // <- �e�X�g�Ŗ��t���[���]������
            //}

            //ForEachAll(actor => actor.StepParams());
            //ForEachAll(actor => actor.StepAction());
        }

        void RegisterActorCallback()
        {
            Actor.OnSpawned += AddSpawnedActorTemp;
            this.OnDisableAsObservable().Subscribe(_ => Actor.OnSpawned -= AddSpawnedActorTemp);
        }

        async UniTask<bool> InitAsync(CancellationToken token)
        {
            // �L�����N�^�[�̃X�e�[�^�X�ǂݍ���
            await StatusBaseHolder.LoadAsync(token);
            // �t�B�[���h�̐���
            Cell[,] field = FieldManager.Instance.Create();
            // ����������z�u
            _initKinpatsuSpawner.Spawn(field);

            return true;
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

        void AddSpawnedActorTemp(Actor actor) => _temp.Enqueue(actor);

        void AddControledActorFromTemp()
        {
            while (_temp.Count > 0)
            {
                Actor actor = _temp.Dequeue();

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