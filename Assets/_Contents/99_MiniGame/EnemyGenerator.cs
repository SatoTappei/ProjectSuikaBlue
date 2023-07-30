using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniGame
{
    public class EnemyGenerator : MonoBehaviour
    {
        [System.Serializable]
        class LevelData
        {
            [SerializeField] GameObject[] _enemyPrefabs;
            [SerializeField] float _timeLimit;
            [SerializeField] float _interval;
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
        [Header("沸きポイントのタグ")]
        [SerializeField] string _spawnPointTag = "SpawnPoint";
        [Header("生成した敵の親")]
        [SerializeField] Transform _parent;

        Transform[] _spawnPoints;
        float _levelTimer;
        float _timer;
        int _currentLevel;
        bool _isValid;

        Vector3 RandomSpawnPointPos => _spawnPoints[Random.Range(0, _spawnPoints.Length)].position;

        void Start()
        {

        }

        void Update()
        {
            if (!_isValid) return;

            _timer += Time.deltaTime;
            if (_timer > _levelData[_currentLevel].Interval)
            {
                _levelTimer += _timer;
                if (_levelTimer > _levelData[_currentLevel].TimeLimit)
                {
                    Debug.Log("れべら");
                    _currentLevel = Mathf.Min(_currentLevel + 1, _levelData.Length - 1);
                    _levelTimer = 0;
                }

                Spawn();

                _timer = 0;

            }
        }

        // 以下2つのメソッドを外部から呼ぶことで生成開始/停止
        public void GenerateStart() { FindSpawnPoints(); _isValid = true; }
        public void GenerateStop() => _isValid = false;

        /// <summary>
        /// 敵の湧きポイントをタグで取得する
        /// </summary>
        void FindSpawnPoints()
        {
            GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag(_spawnPointTag);
            if (spawnPoints.Length == 0)
            {
                throw new System.InvalidOperationException("沸きポイント生成されていない: " + _spawnPointTag);
            }

            _spawnPoints = System.Array.ConvertAll(spawnPoints, g => g.transform);
        }

        /// <summary>
        /// 現在のレベルに応じた敵を生成する
        /// </summary>
        void Spawn()
        {
            for(int i = 0; i < _levelData[_currentLevel].ConcurrentSpawn; i++)
            {
                GameObject prefab = _levelData[_currentLevel].RandomEnemyPrefab;
                GameObject enemy = Instantiate(prefab, RandomSpawnPointPos, Quaternion.identity, _parent);
            }
        }
    }
}
