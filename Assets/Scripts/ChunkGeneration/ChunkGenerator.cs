using System.Collections.Generic;
using ChunkGeneration.Configs;
using Core.ObjectPool;
using Managers;
using UnityEngine;

namespace ChunkGeneration
{
    public class ChunkGenerator : MonoBehaviour
    {
        [SerializeField] private List<ChunkConfig> ordinaryChunkConfigs;
        [SerializeField] private List<ChunkConfig> bossChunkConfigs;
        [SerializeField] private int chunksAhead = 1;
        [SerializeField] private int minChunksBeforeBoss = 5;
        [SerializeField] private int maxChunksBeforeBoss = 7;

        private Queue<GameObject> _activeChunks = new();
        private float _nextSpawnPosition;
        private Transform _playerSpawnSpot;
        private int _chunksSinceLastBoss;
        private int _nextBossAtChunk;

        private void Start()
        {
            CalculateNextBossChunk();
            for (int i = 0; i < chunksAhead; i++)
                SpawnNextChunk();

            if (_playerSpawnSpot != null)
                PlayerManager.Instance.SpawnPlayer(_playerSpawnSpot);
        }

        public void OnPlayerExitedLastChunk()
        {
            if (_activeChunks.Count > chunksAhead)
            {
                GameObject oldestChunk = _activeChunks.Dequeue();
                PoolManager.Instance.Return("chunk", oldestChunk);
            }

            SpawnNextChunk();
        }

        private void CalculateNextBossChunk()
        {
            _nextBossAtChunk = Random.Range(minChunksBeforeBoss, maxChunksBeforeBoss + 1);
            _chunksSinceLastBoss = 0;
        }

        private void SpawnNextChunk()
        {
            _chunksSinceLastBoss++;
            bool spawnBoss = _chunksSinceLastBoss >= _nextBossAtChunk;

            ChunkConfig randomChunk = GetRandomChunkConfig(spawnBoss);
            GameObject newChunk = PoolManager.Instance.Spawn(PoolNames.CHUNK, new Vector3(_nextSpawnPosition, 0, 0), Quaternion.identity);

            ChunkController chunkController = newChunk.GetComponent<ChunkController>();
            chunkController.Initialize(randomChunk, this, spawnBoss);

            _nextSpawnPosition += randomChunk.ChunkLength;
            _activeChunks.Enqueue(newChunk);

            if (spawnBoss)
                CalculateNextBossChunk();

            if (_playerSpawnSpot == null)
                _playerSpawnSpot = newChunk.GetComponent<ChunkController>().GetPlayerSpawnSpot();
        }

        private ChunkConfig GetRandomChunkConfig(bool spawnBoss)
        {
            if (spawnBoss && bossChunkConfigs.Count > 0)
                return bossChunkConfigs[Random.Range(0, bossChunkConfigs.Count)];
        
            return ordinaryChunkConfigs[Random.Range(0, ordinaryChunkConfigs.Count)];
        }
    }
}
