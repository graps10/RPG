using UnityEngine;

namespace Core.ObjectPool.Configs.FX
{
    [CreateAssetMenu(fileName = "ShockStrikePoolConfig", menuName = "ObjectPool/Configs/FX/Shock Strike")]
    public class ShockStrikePoolConfig : BasePoolConfig
    {
        [Header("FX Specific Settings")]
        [SerializeField] private float speed;
        [SerializeField] private float hitDistanceThreshold = 0.1f;
        
        [SerializeField] private float damageDelay = 0.2f;
        [SerializeField] private float returnDelay = 0.4f;
    
        [SerializeField] private Vector2 animLocalPosition = new(0f, 0.5f);
        [SerializeField] private Vector2 hitScale = new(3f, 3f);
        
        public float Speed => speed;
        public float HitDistanceThreshold => hitDistanceThreshold;
        
        public float DamageDelay => damageDelay;
        public float ReturnDelay => returnDelay;
        
        public Vector2 AnimLocalPosition => animLocalPosition;
        public Vector2 HitScale => hitScale;
    }
}