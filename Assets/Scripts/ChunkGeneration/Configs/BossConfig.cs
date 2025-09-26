using UnityEngine;

namespace ChunkGeneration.Configs
{
    [CreateAssetMenu(fileName = "BossChunkConfig", menuName = "Level Generation/Chunk Config/Boss Config")]
    public class BossChunkConfig : ScriptableObject
    {
        [SerializeField] private GameObject chunkPrefab;
        [SerializeField] private float chunkLength = 20f;

        [Header("Enemy Settings")]
        [SerializeField] private EnemySpawnData enemyType;
    }
}

