using UnityEngine;
using System.Collections.Generic;

public class ChunkGenerator : MonoBehaviour
{
    [SerializeField] private List<ChunkConfig> ordinaryChunkConfigs;
    [SerializeField] private List<ChunkConfig> bossChunkConfigs;
    [SerializeField] private int chunksAhead = 1;
    [SerializeField] private int minChunksBeforeBoss = 5;
    [SerializeField] private int maxChunksBeforeBoss = 7;

    private Queue<GameObject> activeChunks = new Queue<GameObject>();
    private float nextSpawnPosition = 0f;
    private Transform playerSpawnSpot;
    private int chunksSinceLastBoss = 0;
    private int nextBossAtChunk;

    void Start()
    {
        CalculateNextBossChunk();
        for (int i = 0; i < chunksAhead; i++)
        {
            SpawnNextChunk();
        }

        if (playerSpawnSpot != null)
            PlayerManager.instance.SpawnPlayer(playerSpawnSpot);
    }

    public void OnPlayerExitedLastChunk()
    {
        if (activeChunks.Count > chunksAhead)
        {
            GameObject oldestChunk = activeChunks.Dequeue();
            PoolManager.instance.Return("chunk", oldestChunk);
        }

        SpawnNextChunk();
    }

    private void CalculateNextBossChunk()
    {
        nextBossAtChunk = Random.Range(minChunksBeforeBoss, maxChunksBeforeBoss + 1);
        chunksSinceLastBoss = 0;
    }

    private void SpawnNextChunk()
    {
        chunksSinceLastBoss++;
        bool spawnBoss = chunksSinceLastBoss >= nextBossAtChunk;

        ChunkConfig randomChunk = GetRandomChunkConfig(spawnBoss);
        GameObject newChunk = PoolManager.instance.Spawn("chunk", new Vector3(nextSpawnPosition, 0, 0), Quaternion.identity);

        ChunkController chunkController = newChunk.GetComponent<ChunkController>();
        chunkController.Initialize(randomChunk, this, spawnBoss);

        nextSpawnPosition += randomChunk.ChunkLength;
        activeChunks.Enqueue(newChunk);

        if (spawnBoss)
            CalculateNextBossChunk();

        if (playerSpawnSpot == null)
            playerSpawnSpot = newChunk.GetComponent<ChunkController>().GetPlayerSpawnSpot();
    }

    private ChunkConfig GetRandomChunkConfig(bool spawnBoss)
    {
        if (spawnBoss && bossChunkConfigs.Count > 0)
        {
            return bossChunkConfigs[Random.Range(0, bossChunkConfigs.Count)];
        }
        return ordinaryChunkConfigs[Random.Range(0, ordinaryChunkConfigs.Count)];
    }
}
