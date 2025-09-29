using System.Collections.Generic;
using UnityEngine;

namespace ChunkGeneration.Configs
{
    [CreateAssetMenu(fileName = "OrdinaryChunkConfig", menuName = "Level Generation/Chunk Config/Ordinary Config")]
    public class OrdinaryConfig : ChunkConfig
    {
        [Space]
        [Header("Ordinary Chunk Enemy Settings")]
        [SerializeField] private int minEnemies;
        [SerializeField] private int maxEnemies;
        [SerializeField] private List<EnemySpawnData> enemiesData;

        public int MinEnemies => minEnemies;
        public int MaxEnemies => maxEnemies;
        public List<EnemySpawnData> EnemiesData => enemiesData;
    }
}

