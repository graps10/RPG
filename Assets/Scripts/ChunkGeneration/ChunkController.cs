using System.Collections;
using System.Collections.Generic;
using ChunkGeneration.Configs;
using ChunkGeneration.Triggers;
using Core.Interfaces;
using Core.ObjectPool;
using Core.ObjectPool.Configs.Enemies;
using Enemies.Base;
using Enemies.DeathBringer;
using UnityEngine;

namespace ChunkGeneration
{
    public class ChunkController : PooledObject
    {
        private const float Check_Chunk_Delay = 1f;
        
        [SerializeField] private Transform playerSpawnSpot;
        [SerializeField] private List<Transform> enemySpawnSpots = new();

        [SerializeField] private ChunkEntryTrigger entryTrigger;
        [SerializeField] private ChunkExitTrigger exitTrigger;

        private ChunkConfig _config;
        private ChunkGenerator _levelGenerator;
        
        private readonly List<Enemy> _spawnedEnemies = new();
        
        private LocationTheme _chunkTheme;
        
        private bool _isBossChunk;
        private bool _chunkCompleted;
        private bool _playerInside;
        
        private Coroutine _checkChunkCoroutine;

        public void Initialize(ChunkConfig config, ChunkGenerator generator, LocationTheme theme, bool isBossChunk)
        {
            _config = config;
            _levelGenerator = generator;
            _chunkTheme = theme;
            _isBossChunk = isBossChunk;
            _chunkCompleted = false;
            _playerInside = false;
            
            SetupEnemies();

            SetupTriggers();
            DeactivateWalls();
        }

        public override void ReturnToPool()
        {
            if (_checkChunkCoroutine != null)
                StopCoroutine(_checkChunkCoroutine);
            
            _chunkCompleted = false;
            _playerInside = false;
            
            foreach (var enemy in _spawnedEnemies)
            {
                if (enemy != null && enemy is IMinionSpawner spawner)
                    spawner.OnMinionSpawned -= HandleMinionSpawned;
            }
            
            ClearAllSpawnedEnemies();
            DeactivateWalls();
            
            base.ReturnToPool();
        }

        #region Triggers
        public void OnPlayerEntry()
        {
            if (_playerInside) return;
            _playerInside = true;
            
            ActivateWalls();
            
            _levelGenerator.DespawnOldestChunk();
            _levelGenerator.UpdateActiveTheme(_chunkTheme);
            
            if (_isBossChunk)
                WakeUpBoss();
            
            if (!_chunkCompleted)
                _checkChunkCoroutine = StartCoroutine(CheckChunkCompletion());
        }
        
        public void OnPlayerExitedLastChunk() => _levelGenerator.OnPlayerExitedLastChunk();
        public void OnPlayerExitedCurrentChunk() { } //Debug.Log("Player exited chunk");

        private void SetupTriggers()
        {
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
            
            if (_spawnedEnemies.Count == 0)
            {
                CompleteChunk();
                return;
            }
            
            _checkChunkCoroutine = StartCoroutine(CheckChunkCompletion());
        }

        private void SpawnOrdinaryEnemies()
        {
            var ordinaryConfig = _config as OrdinaryConfig;
            if (ordinaryConfig == null || ordinaryConfig.EnemiesData.Count == 0) return;
            
            int enemyCount = Random.Range(ordinaryConfig.MinEnemies, ordinaryConfig.MaxEnemies + 1);
            List<Transform> availableSpots = new List<Transform>(enemySpawnSpots);

            for (int i = 0; i < enemyCount && availableSpots.Count > 0; i++)
            {
                int spotIndex = Random.Range(0, availableSpots.Count);
                Transform spot = availableSpots[spotIndex];
                availableSpots.RemoveAt(spotIndex);
                
                EnemyPoolConfig selectedEnemyConfig = GetRandomEnemyByWeight(ordinaryConfig.EnemiesData);
                
                GameObject enemyObj = PoolManager.Instance.Spawn(
                    selectedEnemyConfig.Prefab,
                    spot.position,
                    Quaternion.identity
                );
                
                if (enemyObj.TryGetComponent(out Enemy enemyScript))
                {
                    _spawnedEnemies.Add(enemyScript);
                    if (enemyScript is IMinionSpawner spawner)
                        spawner.OnMinionSpawned += HandleMinionSpawned;
                }
            }
        }
        
        private EnemyPoolConfig GetRandomEnemyByWeight(List<EnemySpawnData> enemiesData)
        {
            float totalWeight = 0f;
            foreach (var data in enemiesData)
            {
                totalWeight += data.SpawnChance;
            }
            
            float randomValue = Random.Range(0f, totalWeight);
            float cumulativeWeight = 0f;
            
            foreach (var data in enemiesData)
            {
                cumulativeWeight += data.SpawnChance;
                
                if (randomValue <= cumulativeWeight)
                {
                    return data.Config;
                }
            }
            
            return enemiesData[0].Config;
        }

        private void SpawnEnemyBoss()
        {
            var bossConfig = _config as BossChunkConfig;
            if (bossConfig == null) return;
            
            if (bossConfig.BossData != null)
            {
                Transform bossSpot = enemySpawnSpots[0];
                GameObject bossObj = PoolManager.Instance.Spawn(
                    bossConfig.BossData.Config.Prefab,
                    bossSpot.position,
                    Quaternion.identity
                );
                
                if (bossObj.TryGetComponent(out Enemy bossScript))
                    _spawnedEnemies.Add(bossScript);
            }
        }
        
        private void HandleMinionSpawned(Enemy newMinion)
        {
            if (!_spawnedEnemies.Contains(newMinion))
            {
                _spawnedEnemies.Add(newMinion);
                if (newMinion is IMinionSpawner spawner)
                    spawner.OnMinionSpawned += HandleMinionSpawned;
            }
        }
        
        private void WakeUpBoss()
        {
            foreach (var enemyObj in _spawnedEnemies)
            {
                if (enemyObj.TryGetComponent(out EnemyDeathBringer boss))
                {
                    boss.StartBattle();
                    break;
                }
            }
        }

        private void ClearAllSpawnedEnemies()
        {
            if (_spawnedEnemies == null) return;

            foreach (var enemy in _spawnedEnemies)
            {
                if (enemy != null && enemy.gameObject.activeInHierarchy)
                    PoolManager.Instance.Return(enemy.gameObject);
            }

            _spawnedEnemies.Clear();
        }
        
        private IEnumerator CheckChunkCompletion()
        {
            while (!_chunkCompleted)
            {
                _spawnedEnemies.RemoveAll(enemy => enemy == null 
                                                   || !enemy.gameObject.activeInHierarchy 
                                                   || enemy.Stats.IsDead);

                if (_spawnedEnemies.Count == 0)
                    CompleteChunk();

                yield return new WaitForSeconds(Check_Chunk_Delay);
            }
        }
        
        #endregion
        
        private void CompleteChunk()
        {
            _chunkCompleted = true;
            
            DeactivateWalls();
            
            _levelGenerator.SpawnNextChunk();
        }

        private void ActivateWalls()
        {
            entryTrigger.EnablePhysicalWall();
            exitTrigger.EnablePhysicalWall();
        }

        private void DeactivateWalls()
        {
            entryTrigger.DisablePhysicalWall();
            exitTrigger.DisablePhysicalWall();
        }
        
        public Transform GetPlayerSpawnSpot() => playerSpawnSpot;
        
        #region Editor Debug

#if UNITY_EDITOR
        [Header("Editor Debug")]
        [SerializeField] private ChunkConfig debugConfig;

        private void OnDrawGizmos()
        {
            if (debugConfig == null) return;
    
            Vector3 centerPos = transform.position;
            float halfLength = debugConfig.ChunkLength / 2f;
            
            Vector3 leftPos = centerPos - new Vector3(halfLength, 0, 0);
            Vector3 rightPos = centerPos + new Vector3(halfLength, 0, 0);
            
            Gizmos.color = Color.green;
            Gizmos.DrawLine(leftPos, rightPos);
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(leftPos + Vector3.down * 10f, leftPos + Vector3.up * 10f);
            Gizmos.DrawLine(rightPos + Vector3.down * 10f, rightPos + Vector3.up * 10f);
        }
#endif

        #endregion
    }
}
