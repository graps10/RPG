using UnityEngine;

namespace Core.ObjectPool.Configs.FX
{
    [CreateAssetMenu(fileName = "DotPoolConfig", menuName = "ObjectPool/Configs/FX/Aim Dot")]
    public class DotPoolConfig : BasePoolConfig
    {
        [Header("Dots Specific Settings")]
        [SerializeField] private int numberOfDots;
        [SerializeField] private float spaceBetweenDots;
        
        public int NumberOfDots => numberOfDots;
        public float SpaceBetweenDots => spaceBetweenDots;
    }
}