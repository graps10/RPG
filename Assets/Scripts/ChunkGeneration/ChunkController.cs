using System.Collections;
using System.Collections.Generic;
using ChunkGeneration.Configs;
using ChunkGeneration.Triggers;
using Core.ObjectPool;
using UnityEngine;

namespace ChunkGeneration
{
    public class ChunkController : MonoBehaviour, IPooledObject
    {
        private const float Check_Chunk_Delay = 1f;
        
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
        
        private Coroutine _checkChunkCoroutine;

        public void Initialize(ChunkConfig config, ChunkGenerator generator, bool isBossChunk)
        {
            _config = config;
            _levelGenerator = generator;
            _isBossChunk = isBossChunk;

            UnlockDoors();
            SetupEnemies();
            SetupTriggers();
        }

        public void OnReturnToPool()
        {
            if (_checkChunkCoroutine != null)
                StopCoroutine(_checkChunkCoroutine);
            
            _chunkCompleted = false;
            transform.position = Vector3.zero;
            
            ClearAllSpawnedEnemies();
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

        #region Enemies
        
        private void SetupEnemies()
        {
            if (enemySpawnSpots.Count == 0)
                return;

            if (_isBossChunk)
                SpawnEnemyBoss();
            else
                SpawnOrdinaryEnemies();
            
            _checkChunkCoroutine = StartCoroutine(CheckChunkCompletion());
        }

        private void SpawnOrdinaryEnemies()
        {
            var ordinaryConfig = _config as OrdinaryConfig;
            if (ordinaryConfig == null) return;

            int enemyCount = Random.Range(ordinaryConfig.MinEnemies, ordinaryConfig.MaxEnemies + 1);
            List<Transform> availableSpots = new List<Transform>(enemySpawnSpots);

            for (int i = 0; i < enemyCount && availableSpots.Count > 0; i++)
            {
                int spotIndex = Random.Range(0, availableSpots.Count);
                Transform spot = availableSpots[spotIndex];
                availableSpots.RemoveAt(spotIndex);

                foreach (var enemyType in ordinaryConfig.EnemiesData)
                {
                    if (Random.value <= enemyType.SpawnChance)
                    {
                        GameObject enemy = Instantiate(
                            enemyType.EnemyPrefab,
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

        private void SpawnEnemyBoss()
        {
            var bossConfig = _config as BossChunkConfig;
            if (bossConfig == null) return;
            
            if (bossConfig.BossData != null)
            {
                Transform bossSpot = enemySpawnSpots[0];
                GameObject boss = Instantiate(
                    bossConfig.BossData.EnemyPrefab,
                    bossSpot.position,
                    Quaternion.identity,
                    bossSpot
                );
                
                _spawnedEnemies.Add(boss);
            }
        }

        private void ClearAllSpawnedEnemies()
        {
            foreach (var enemy in _spawnedEnemies)
                if (enemy != null)
                    Destroy(enemy);
            
            _spawnedEnemies.Clear();
        }
        
        private IEnumerator CheckChunkCompletion()
        {
            while (!_chunkCompleted)
            {
                if (_spawnedEnemies.Count > 0)
                {
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
                    {
                        CompleteChunk();
                        yield break; 
                    }
                }

                yield return new WaitForSeconds(Check_Chunk_Delay);
            }
        }
        
        #endregion
        
        private void CompleteChunk()
        {
            _chunkCompleted = true;
            UnlockDoors();
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
        
        public Transform GetPlayerSpawnSpot() => playerSpawnSpot;
    }
}
