using UnityEngine;

namespace Core.ObjectPool.Configs.FX
{
    [CreateAssetMenu(fileName = "AfterImagePoolConfig", menuName = "ObjectPool/Configs/FX/After Image")]
    public class AfterImagePoolConfig : BasePoolConfig
    {
        [Header("After Image Specific Settings")]
        [SerializeField] private float colorLoseRate;
        [SerializeField] private float afterImageCooldown;
        
        public float ColorLoseRate =>  colorLoseRate;
        public float AfterImageCooldown =>  afterImageCooldown;
    }
}