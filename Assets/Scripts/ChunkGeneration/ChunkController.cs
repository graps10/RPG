using System;
using System.Collections.Generic;
using UnityEngine;

public class ChunkController : MonoBehaviour, IReturnedObject
{
    [SerializeField] private Transform playerSpawnSpot;
    [SerializeField] private List<Transform> enemySpawnSpots = new List<Transform>();

    [SerializeField] private ChunkEntryTrigger entryTrigger;
    [SerializeField] private ChunkExitTrigger exitTrigger;
    [SerializeField] private ChunkGenerationTrigger generationTrigger;

    private ChunkConfig config;
    private ChunkGenerator levelGenerator;

    private List<GameObject> spawnedEnemies = new List<GameObject>();
    private bool isBossChunk;
    private bool chunkCompleted = false;

    public void Initialize(ChunkConfig _config, ChunkGenerator _generator, bool _isBossChunk)
    {
        config = _config;
        levelGenerator = _generator;

        isBossChunk = _isBossChunk;

        UnlockDoors();
        SetupEnemies();
        SetupTriggers();
    }

    void Update()
    {
        if (!chunkCompleted && spawnedEnemies.Count > 0)
        {
            bool allDead = true;
            foreach (var enemy in spawnedEnemies)
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
    }

    public Transform GetPlayerSpawnSpot() => playerSpawnSpot;

    public void OnReturnToPool()
    {
        chunkCompleted = false;
        transform.position = Vector3.zero;
        ClearEnemies();
        UnlockDoors();
    }

    #region Triggers
    public void OnPlayerEntry() => LockDoors();
    public void OnPlayerExitedLastChunk() => levelGenerator.OnPlayerExitedLastChunk();
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
        chunkCompleted = true;
        UnlockDoors();
    }

    private void SetupEnemies()
    {
        if (enemySpawnSpots.Count == 0)
            return;

        if (isBossChunk)
            SpawnBoss();
        else
            SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        int enemyCount = UnityEngine.Random.Range(config.MinEnemies, config.MaxEnemies + 1);
        List<Transform> availableSpots = new List<Transform>(enemySpawnSpots);

        for (int i = 0; i < enemyCount && availableSpots.Count > 0; i++)
        {
            int spotIndex = UnityEngine.Random.Range(0, availableSpots.Count);
            Transform spot = availableSpots[spotIndex];
            availableSpots.RemoveAt(spotIndex);

            foreach (var enemyType in config.EnemyTypes)
            {
                if (UnityEngine.Random.value <= enemyType.spawnChance)
                {
                    GameObject enemy = Instantiate(
                        enemyType.prefab,
                        spot.position,
                        Quaternion.identity,
                        spot
                    );
                    spawnedEnemies.Add(enemy);
                    break;
                }
            }
        }
    }

    private void SpawnBoss()
    {
        if (config.BossPrefab != null)
        {
            Transform bossSpot = enemySpawnSpots[0];
            GameObject boss = Instantiate(
                config.BossPrefab,
                bossSpot.position,
                Quaternion.identity,
                bossSpot
            );
            spawnedEnemies.Add(boss);
        }
    }

    private void ClearEnemies()
    {
        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        spawnedEnemies.Clear();
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
