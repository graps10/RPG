using System.Collections.Generic;
using UnityEngine;

namespace ChunkGeneration.Configs
{
    [CreateAssetMenu(fileName = "OrdinaryChunkConfig", menuName = "Level Generation/Chunk Config/Ordinary Config")]
    public class OrdinaryConfig : ScriptableObject
    {
        [Header("Enemy Settings")]
        [SerializeField] private int minEnemies = 1;
        [SerializeField] private int maxEnemies = 3;
        [SerializeField] private List<EnemySpawnData> enemyTypes;

        public int MinEnemies => minEnemies;
        public int MaxEnemies => maxEnemies;
        public List<EnemySpawnData> EnemyTypes => enemyTypes;
    }
}

