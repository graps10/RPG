using Core.ObjectPool.Configs;
using Core.ObjectPool.Configs.Enemies;
using UnityEngine;

namespace ChunkGeneration.Configs
{
    [CreateAssetMenu(fileName = "ChunkConfig", menuName = "Level Generation/Chunk Config")]
    public class ChunkConfig : BasePoolConfig
    {
        [SerializeField] private float chunkLength;
        
        public float ChunkLength => chunkLength;
    }
    
    [System.Serializable]
    public class EnemySpawnData
    {
        public EnemyPoolConfig Config;
        [Range(0f, 1f)] public float SpawnChance;
    }
}