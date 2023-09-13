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
        [SerializeField] KurokamiSpawnModule _kurokamiSpawnModule;

        LeaderSelector _leaderSelector = new();
        Actor _leader;
        List<Actor> _kinpatsuList = new();
        List<Actor> _kurokamiList = new();

        bool _initialized;
        // �C�ӂ̃^�C�~���O�ŃL�����N�^�[�̃��X�g���X�V���邽�߂̈ꎞ�ۑ��p�̃L���[
        Queue<Actor> _temp = new();
        // ���[�_�[�����Ȃ��ꍇ�̃_�~�[�̕]���z��
        float[] _dummyEvaluate = new float[Utility.GetEnumLength<ActionType>() - 1];

        async void Start()
        {
            _initialized = await InitAsync(this.GetCancellationTokenOnDestroy());
        }

        void Update()
        {
            if (!_initialized) return;

            // ���䂷��L�����N�^�[�̃��X�g�̍X�V
            AddControledActorFromTemp();

            // ���񂾃L�����N�^�[�����X�g����폜
            if (_leader != null && _leader.IsDead) _leader = null;
            _kinpatsuList.RemoveAll(actor => actor.IsDead);
            _kurokamiList.RemoveAll(actor => actor.IsDead);

            // �S�L�����N�^�[����
            ForEachAll(actor => actor.StepParams());
            ForEachAll(actor => actor.StepAction());
            ForEachEvaluate(actor => actor.ResetOnEvaluateState());
            // ���[�_�[�̕]�������ɋ����̕]�����s��
            float[] leaderEval = _leader != null ? _leader.LeaderEvaluate() : _dummyEvaluate;
            foreach (Actor kinpatsu in _kinpatsuList.Where(a => a.State == StateType.Evaluate))
            {
                kinpatsu.Evaluate(leaderEval);
            }
            // �����̕]���̓��[�_�[�����Ȃ��̂Ń_�~�[�̔z����g�p
            foreach (Actor kurokami in _kurokamiList.Where(a => a.State == StateType.Evaluate))
            {
                kurokami.Evaluate(_dummyEvaluate);
            }

            // ���Ԋu�ō����𐶐�����
            _kurokamiSpawnModule.StepSpawnFromCandidate(_kinpatsuList);
            // ���Ԋu�Ń��[�_�[��I�o����
            if (_leaderSelector.StepTryLeaderSelect(_kinpatsuList, out Actor nextLeader))
            {
                _leader = nextLeader;
            }
        }

        void OnDestroy()
        {
            StatusBaseHolder.Release();
            PublicBlackBoard.Release();
        }

        async UniTask<bool> InitAsync(CancellationToken token)
        {
            RegisterActorCallback();
            // �L�����N�^�[�̃X�e�[�^�X�ǂݍ���
            await StatusBaseHolder.LoadAsync(token);
            // �t�B�[���h�̐���
            Cell[,] field = FieldManager.Instance.Create();
            // ����������z�u
            _initKinpatsuSpawner.Spawn(field);

            return true;
        }

        /// <summary>
        /// ���[�v���ɃL�����N�^�[�̐����������ă��X�g�̗v�f�����ς�鎖��h�����߂�
        /// �L�����N�^�[�������Ɉꎞ�ۑ��p�̃L���[�ɒǉ�����
        /// </summary>
        void RegisterActorCallback()
        {
            Actor.OnSpawned += actor => _temp.Enqueue(actor);
            this.OnDisableAsObservable().Subscribe(_ => Actor.OnSpawned -= actor => _temp.Enqueue(actor));
        }

        /// <summary>
        /// �ꎞ�ۑ��p�̃L���[���g��S�Đ��䂷��L�����N�^�[�̃��X�g�ɒǉ�����
        /// </summary>
        void AddControledActorFromTemp()
        {
            while (_temp.Count > 0)
            {
                Actor actor = _temp.Dequeue();

                if (actor.Type == ActorType.KinpatsuLeader) _leader = actor;
                else if (actor.Type == ActorType.Kinpatsu) _kinpatsuList.Add(actor);
                else if (actor.Type == ActorType.Kurokami) _kurokamiList.Add(actor);
                else
                {
                    string msg = "�L�����N�^�[�̎�ނ�None�Ȃ̂�Controller�Ő���s�\: " + actor.name;
                    throw new System.ArgumentException(msg);
                }
            }
        }

        void ForEachAll(UnityAction<Actor> action)
        {
            foreach (Actor kinpatsu in _kinpatsuList)
            {
                action.Invoke(kinpatsu);
            }
            foreach (Actor kurokami in _kurokamiList)
            {
                action.Invoke(kurokami);
            }
        }

        void ForEachEvaluate(UnityAction<Actor> action)
        {
            foreach (Actor kinpatsu in _kinpatsuList.Where(a => a.State == StateType.Evaluate))
            {
                action.Invoke(kinpatsu);
            }
            foreach (Actor kurokami in _kurokamiList.Where(a => a.State == StateType.Evaluate))
            {
                action.Invoke(kurokami);
            }
        }

        // �f�o�b�O�p
        void DebugLog()
        {
            Debug.Log($"���[�_�[:{_leader} ����:{_kinpatsuList.Count} ����:{_kurokamiList.Count}");
        }
    }
}