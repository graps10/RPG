using System;
using System.Collections.Generic;
using ChunkGeneration.Configs;
using Managers;
using UnityEngine;
using PoolManager = Core.ObjectPool.PoolManager;

namespace ChunkGeneration
{
    public class ChunkGenerator : MonoBehaviour
    {
        private const int Max_Spawned_Chunks_Count = 2;
        
        [Header("World Progression")]
        [Tooltip("Valley, Forest, Castle")]
        [SerializeField] private List<LocationConfig> locations;

        [Header("Generation Rules")]
        [SerializeField] private int chunksAhead = 1;
        [SerializeField] private float chunkGap = 0f;

        public static int LoopCount { get; private set; } = 0;
        public static float DifficultyMultiplier => 1f + (LoopCount * 0.5f);

        public static event Action<LocationTheme> OnThemeChanged;

        private int _currentLocationIndex;
        private LocationTheme _activeTheme;

        private Queue<GameObject> _activeChunks = new();
        private float _nextSpawnPosition;
        private Transform _playerSpawnSpot;

        private int _chunksSpawnedInCurrentTheme;
        private bool _isSpawningBoss;

        private void Start()
        {
            if (locations == null || locations.Count == 0)
            {
                Debug.LogError("No locations assigned to ChunkGenerator!");
                return;
            }

            _activeTheme = locations[0].Theme;
            
            SpawnNextChunk();
            if (_activeChunks.Count > 0)
            {
                GameObject firstChunk = _activeChunks.Peek();
                firstChunk.GetComponent<ChunkController>().OnPlayerEntry();
            }
            
            if (_playerSpawnSpot != null)
                PlayerManager.Instance.TeleportPlayer(_playerSpawnSpot.position);
        }

        public void OnPlayerExitedLastChunk()
        {
            if (_activeChunks.Count > chunksAhead)
            {
                GameObject oldestChunk = _activeChunks.Dequeue();
                PoolManager.Instance.Return(oldestChunk);
            }

            SpawnNextChunk();
        }

        public void SpawnNextChunk()
        {
            LocationConfig currentLocation = locations[_currentLocationIndex];
            ChunkConfig chunkToSpawn = null;
            bool spawningBossChunk = false;

            if (_isSpawningBoss)
            {
                chunkToSpawn = currentLocation.GetBossChunk(LoopCount);
                spawningBossChunk = true;
                _isSpawningBoss = false;

                AdvanceToNextLocation();
            }
            else
            {
                chunkToSpawn = currentLocation.GetOrdinaryChunk(_chunksSpawnedInCurrentTheme);
                _chunksSpawnedInCurrentTheme++;

                if (_chunksSpawnedInCurrentTheme >= currentLocation.ChunksPerTheme)
                {
                    if (currentLocation.HasBoss)
                        _isSpawningBoss = true;
                    else
                        AdvanceToNextLocation();
                }
            }

            if (chunkToSpawn == null)
            {
                Debug.LogError($"No valid chunk found for location {currentLocation.Theme}! Check your configs.");
                return;
            }

            float halfLength = chunkToSpawn.ChunkLength / 2f;
            float spawnX = _nextSpawnPosition + halfLength;

            Vector3 spawnPosition = new Vector3(spawnX, 0, 0);
            
            GameObject newChunk = PoolManager.Instance.Spawn(chunkToSpawn.Prefab, spawnPosition, Quaternion.identity);

            ChunkController chunkController = newChunk.GetComponent<ChunkController>();
            chunkController.Initialize(chunkToSpawn, this, currentLocation.Theme, spawningBossChunk);
            
            _nextSpawnPosition = spawnX + halfLength + chunkGap;
            
            _activeChunks.Enqueue(newChunk);

            if (_playerSpawnSpot == null)
                _playerSpawnSpot = chunkController.GetPlayerSpawnSpot();
        }
        
        public void DespawnOldestChunk()
        {
            if (_activeChunks.Count > Max_Spawned_Chunks_Count) 
            {
                GameObject oldestChunk = _activeChunks.Dequeue();
                PoolManager.Instance.Return(oldestChunk);
            }
        }

        private void AdvanceToNextLocation()
        {
            _chunksSpawnedInCurrentTheme = 0;
            _currentLocationIndex++;

            if (_currentLocationIndex >= locations.Count)
            {
                _currentLocationIndex = 0;
                LoopCount++;
                Debug.Log($"Loop {LoopCount} started! Difficulty is now x{DifficultyMultiplier}");
            }
        }

        public void UpdateActiveTheme(LocationTheme newTheme)
        {
            if (_activeTheme != newTheme)
            {
                _activeTheme = newTheme;
                OnThemeChanged?.Invoke(_activeTheme);
            }
        }
    }
}