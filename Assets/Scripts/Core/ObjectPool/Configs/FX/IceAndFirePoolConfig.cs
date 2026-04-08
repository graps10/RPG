using UnityEngine;

namespace Core.ObjectPool.Configs.FX
{
    [CreateAssetMenu(fileName = "IceAndFirePoolConfig", menuName = "ObjectPool/Configs/FX/Ice And Fire FX")]
    public class IceAndFirePoolConfig : BasePoolConfig
    {
        [Header("FX Specific Settings")]
        [SerializeField] private float returnToPoolDelay = 10f;
        
        public float ReturnToPoolDelay => returnToPoolDelay;
    }
}