using UnityEngine;

namespace Core.ObjectPool.Configs.FX
{
    [CreateAssetMenu(fileName = "ThunderStrikePoolConfig", menuName = "ObjectPool/Configs/FX/Thunder Strike")]
    public class ThunderStrikePoolConfig : BasePoolConfig
    {
        [Header("FX Specific Settings")]
        [SerializeField] private float returnToPoolDelay = 1f;
        
        public float ReturnToPoolDelay => returnToPoolDelay;
    }
}