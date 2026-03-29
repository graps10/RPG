using System.Collections.Generic;
using UnityEngine;

namespace ChunkGeneration.Configs
{
    public enum LocationTheme { Valley, Forest, Castle }
    
    [CreateAssetMenu(fileName = "New Location", menuName = "Level Generation/Location Config")]
    public class LocationConfig : ScriptableObject
    {
        [Header("Location Settings")]
        [SerializeField] private LocationTheme theme;
        [SerializeField] private int chunksPerTheme = 3; 

        [Header("Chunks Pool")]
        [SerializeField] private List<ChunkConfig> ordinaryChunks = new();
        [SerializeField] private List<BossChunkConfig> bossChunks = new();
        
        public LocationTheme Theme => theme;
        public int ChunksPerTheme => chunksPerTheme;
        
        public bool HasBoss => bossChunks != null && bossChunks.Count > 0;

        public ChunkConfig GetRandomOrdinaryChunk()
        {
            if (ordinaryChunks.Count == 0) return null;
            return ordinaryChunks[Random.Range(0, ordinaryChunks.Count)];
        }

        public BossChunkConfig GetRandomBossChunk()
        {
            if (bossChunks.Count == 0) return null;
            return bossChunks[Random.Range(0, bossChunks.Count)];
        }
    }
}