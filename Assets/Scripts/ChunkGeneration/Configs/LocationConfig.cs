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

        public ChunkConfig GetOrdinaryChunk(int index)
        {
            if (ordinaryChunks.Count == 0) return null;
            return ordinaryChunks[index % ordinaryChunks.Count];
        }
        
        public BossChunkConfig GetBossChunk(int index)
        {
            if (bossChunks.Count == 0) return null;
            return bossChunks[index % bossChunks.Count];
        }
    }
}