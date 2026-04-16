using Core.ObjectPool.Configs.FX;
using UnityEngine;

namespace Core.ObjectPool.Configs.Enemies
{
    [CreateAssetMenu(fileName = "ShadyPoolConfig", menuName = "ObjectPool/Configs/Enemies/Shady")]
    public class ShadyPoolConfig : EnemyPoolConfig
    {
        [Header("Shady Specific Settings")]
        [SerializeField] private float battleStateMoveSpeed;
        [SerializeField] private ExplosivePoolConfig explosivePoolConfig;
        [SerializeField] private float growSpeed;
        [SerializeField] private float maxSize;
        
        public float BattleStateMoveSpeed =>  battleStateMoveSpeed;
        public ExplosivePoolConfig ExplosiveConfig => explosivePoolConfig;
        public float GrowSpeed => growSpeed;
        public float MaxSize => maxSize;
    }
}