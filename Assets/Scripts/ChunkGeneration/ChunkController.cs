using System.Collections.Generic;
using ChunkGeneration.Configs;
using ChunkGeneration.Triggers;
using Core.ObjectPool;
using UnityEngine;

namespace ChunkGeneration
{
    public class ChunkController : MonoBehaviour, IPooledObject
    {
        [SerializeField] private Transform playerSpawnSpot;
        [SerializeField] private List<Transform> enemySpawnSpots = new();

        [SerializeField] private ChunkEntryTrigger entryTrigger;
        [SerializeField] private ChunkExitTrigger exitTrigger;
        [SerializeField] private ChunkGenerationTrigger generationTrigger;

        private ChunkConfig _config;
        private ChunkGenerator _levelGenerator;

        private List<GameObject> _spawnedEnemies = new();
        private bool _isBossChunk;
        private bool _chunkCompleted;

        public void Initialize(ChunkConfig config, ChunkGenerator generator, bool isBossChunk)
        {
            _config = config;
            _levelGenerator = generator;
            _isBossChunk = isBossChunk;

            UnlockDoors();
            SetupEnemies();
            SetupTriggers();
        }

        private void Update()
        {
            if (_chunkCompleted || _spawnedEnemies.Count <= 0) 
                return;
        
            bool allDead = true;
            foreach (var enemy in _spawnedEnemies)
            {
                if (enemy != null)
                {
                    allDead = false;
                    break;
                }
            }

            if (allDead)
                CompleteChunk();
        }

        public Transform GetPlayerSpawnSpot() => playerSpawnSpot;

        public void OnReturnToPool()
        {
            _chunkCompleted = false;
            transform.position = Vector3.zero;
            ClearEnemies();
            UnlockDoors();
        }

        #region Triggers
        public void OnPlayerEntry() => LockDoors();
        public void OnPlayerExitedLastChunk() => _levelGenerator.OnPlayerExitedLastChunk();
        public void OnPlayerExitedCurrentChunk() => Debug.Log("Player exited chunk");

        private void SetupTriggers()
        {
            generationTrigger.Initialize(this);
            entryTrigger.Initialize(this);
            exitTrigger.Initialize(this);
        }

        #endregion

        private void CompleteChunk()
        {
            _chunkCompleted = true;
            UnlockDoors();
        }

        private void SetupEnemies()
        {
            if (enemySpawnSpots.Count == 0)
                return;

            if (_isBossChunk)
                SpawnBoss();
            else
                SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            int enemyCount = Random.Range(_config.MinEnemies, _config.MaxEnemies + 1);
            List<Transform> availableSpots = new List<Transform>(enemySpawnSpots);

            for (int i = 0; i < enemyCount && availableSpots.Count > 0; i++)
            {
                int spotIndex = Random.Range(0, availableSpots.Count);
                Transform spot = availableSpots[spotIndex];
                availableSpots.RemoveAt(spotIndex);

                foreach (var enemyType in _config.EnemyTypes)
                {
                    if (Random.value <= enemyType.spawnChance)
                    {
                        GameObject enemy = Instantiate(
                            enemyType.prefab,
                            spot.position,
                            Quaternion.identity,
                            spot
                        );
                        _spawnedEnemies.Add(enemy);
                        break;
                    }
                }
            }
        }

        private void SpawnBoss()
        {
            if (_config.BossPrefab != null)
            {
                Transform bossSpot = enemySpawnSpots[0];
                GameObject boss = Instantiate(
                    _config.BossPrefab,
                    bossSpot.position,
                    Quaternion.identity,
                    bossSpot
                );
                _spawnedEnemies.Add(boss);
            }
        }

        private void ClearEnemies()
        {
            foreach (var enemy in _spawnedEnemies)
            {
                if (enemy != null)
                    Destroy(enemy);
            }
            
            _spawnedEnemies.Clear();
        }

        private void LockDoors()
        {
            entryTrigger.DisableTrigger();
            exitTrigger.DisableTrigger();
        }

        private void UnlockDoors()
        {
            entryTrigger.EnableTrigger();
            exitTrigger.EnableTrigger();
        }
    }
}
