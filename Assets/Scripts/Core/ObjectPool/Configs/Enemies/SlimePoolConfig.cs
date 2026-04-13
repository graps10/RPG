using Enemies.Slime;

namespace Core.ObjectPool.Configs.Enemies
{
    using UnityEngine;

    namespace Core.ObjectPool.Configs
    {
        [CreateAssetMenu(fileName = "SlimePoolConfig", menuName = "ObjectPool/Configs/Enemies/Slime")]
        public class SlimePoolConfig : EnemyPoolConfig
        {
            [Header("Slime Specific Settings")]
            [SerializeField] private SlimeType slimeType;
            
            [SerializeField] private SlimePoolConfig childSlimeConfig;
            [SerializeField] private int slimesToCreate;
            
            [SerializeField] private Vector2 minCreationVelocity;
            [SerializeField] private Vector2 maxCreationVelocity;
            [Space]
            [SerializeField] private float returnToPoolDelay = 1f; 

            public SlimeType SlimeType => slimeType;

            public SlimePoolConfig ChildSlimeConfig => childSlimeConfig;
            public int SlimesToCreate => slimesToCreate;
            
            public Vector2 MinCreationVelocity => minCreationVelocity;
            public Vector2 MaxCreationVelocity => maxCreationVelocity;
            
            public float ReturnToPoolDelay => returnToPoolDelay;
        }
    }
}