using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public class EnemySpawner : MonoBehaviour
    {
        [System.Serializable]
        class LevelData
        {
            [SerializeField] GameObject[] _enemyPrefabs;
            [SerializeField] float _timeLimit;
            [Min(1.0f)]
            [SerializeField] float _interval;
            [Range(1, 5)]
            [SerializeField] int _concurrentSpawn;

            public GameObject RandomEnemyPrefab
            {
                get => _enemyPrefabs[Random.Range(0, _enemyPrefabs.Length)];
            }
            public float TimeLimit => _timeLimit;
            public float Interval => _interval;
            public int ConcurrentSpawn => _concurrentSpawn;
        }

        [SerializeField] LevelData[] _levelData;

        Transform _parent;
        Transform[] _spawnPoints;
        float _spawnTimer;
        float _levelTimer;
        int _currentLevel;
        bool _isValid;

        void Awake()
        {
            _parent = new GameObject("EnemyParent").transform;
        }

        void Update()
        {
            if (_isValid)
            {
                StepSpawn();
                StepLevelUp();
            }
        }

        // �ȉ�2�̃��\�b�h���O������ĂԂ��ƂŐ����J�n/��~
        public void GenerateStart() { FindSpawnPoints(); _isValid = true; }
        public void GenerateStop() { ResetLevel(); _isValid = false; }

        void ResetLevel()
        {
            _spawnTimer = 0;
            _levelTimer = 0;
            _currentLevel = 0;
        }

        /// <summary>
        /// ���݂̃��x���̊Ԋu�ň��Ԋu�œG�𐶐�����
        /// </summary>
        void StepSpawn()
        {
            _spawnTimer += Time.deltaTime;
            if (_spawnTimer > _levelData[_currentLevel].Interval)
            {
                Spawn();
                _spawnTimer = 0;
            }
        }

        /// <summary>
        /// ���Ԋu�Ń��x�����グ�āA��Փx�A�b�v
        /// </summary>
        void StepLevelUp()
        {
            _levelTimer += Time.deltaTime;
            if (_levelTimer > _levelData[_currentLevel].TimeLimit)
            {
                _currentLevel = Mathf.Min(_currentLevel + 1, _levelData.Length - 1);
                _levelTimer = 0;
            }
        }

        /// <summary>
        /// �G�̗N���|�C���g���^�O�Ŏ擾����
        /// </summary>
        void FindSpawnPoints()
        {
            GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag(TagUtility.SpawnPointTag);
            if (spawnPoints.Length == 0)
            {
                throw new System.InvalidOperationException("�����|�C���g��������Ă��Ȃ�: " + TagUtility.SpawnPointTag);
            }

            _spawnPoints = System.Array.ConvertAll(spawnPoints, g => g.transform);
        }

        /// <summary>
        /// ���݂̃��x���ɉ������G�𐶐�����
        /// �����ő�ʂɐ��������A�����̓G�𐶐�����\��������̂Ńv�[�����O�ł͂Ȃ�����������
        /// </summary>
        void Spawn()
        {
            // �����ӏ��̏d����h�����߂ɁA�����_���ȓY�����̔z��
            int[] randomArray = MyUtility.Utility.DurstenfeldShuffle(_spawnPoints.Length);
            for(int i = 0; i < _levelData[_currentLevel].ConcurrentSpawn; i++)
            {
                GameObject prefab = _levelData[_currentLevel].RandomEnemyPrefab;
                int index = randomArray[i];
                Vector3 spawnPos = _spawnPoints[index].position;
                GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity, _parent);
            }
        }
    }
}
