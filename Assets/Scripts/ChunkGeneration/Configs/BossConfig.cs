using UnityEngine;

namespace ChunkGeneration.Configs
{
    [CreateAssetMenu(fileName = "BossChunkConfig", menuName = "Level Generation/Chunk Config/Boss Config")]
    public class BossChunkConfig : ChunkConfig
    {
        [SerializeField] private EnemySpawnData bossData;
        
        public EnemySpawnData BossData => bossData;
    }
}

