using System.Collections.Generic;
using UnityEngine;

namespace ChunkGeneration.Configs
{
    [CreateAssetMenu(fileName = "ChunkConfig", menuName = "Level Generation/Chunk Config")]
    public class ChunkConfig : ScriptableObject
    {
        [Space]
        [SerializeField] private GameObject chunkPrefab;
        [SerializeField] private float chunkLength;
        
        public float ChunkLength => chunkLength;
    }


    
    [System.Serializable]
    public class EnemySpawnData
    {
        public GameObject EnemyPrefab;
        [Range(0f, 1f)] public float SpawnChance;
    }
}