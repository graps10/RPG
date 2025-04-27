using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChunkConfig", menuName = "Level Generation/Chunk Config")]
public class ChunkConfig : ScriptableObject
{
    [Header("Chunk Settings")]
    [SerializeField] private ChunkType type;

    [Space]
    [SerializeField] private GameObject chunkPrefab;
    [SerializeField] private float chunkLength = 20f;

    [Header("Ordinary Chunk Enemy Settings")]
    [SerializeField] private int minEnemies = 1;
    [SerializeField] private int maxEnemies = 3;
    [SerializeField] private List<EnemySpawnData> enemyTypes;

    [Header("Boss Chunk Settings")]
    [SerializeField] private GameObject bossPrefab;

    public ChunkType Type => type;
    public GameObject ChunkPrefab => chunkPrefab;
    public float ChunkLength => chunkLength;

    public int MinEnemies => minEnemies;
    public int MaxEnemies => maxEnemies;
    public List<EnemySpawnData> EnemyTypes => enemyTypes;

    public GameObject BossPrefab => bossPrefab;
}


public enum ChunkType { Ordinary, Boss }

[System.Serializable]
public class EnemySpawnData
{
    public GameObject prefab;
    [Range(0f, 1f)] public float spawnChance = 0.5f;
}

