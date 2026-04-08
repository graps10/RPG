using UnityEngine;

namespace Core.ObjectPool.Configs
{
    [CreateAssetMenu(fileName = "ArrowsPoolConfig", menuName = "ObjectPool/Configs/Arrow")]
    public class ArrowPoolConfig : BasePoolConfig
    {
        [Header("Arrow Specific Settings")]
        [SerializeField] private int damage;
        [SerializeField] private float speed;
        [SerializeField] private float hitReturnDelay = 6f;  
        [SerializeField] private float missReturnDelay = 15f;
        [SerializeField] private LayerMask whatIsPlayer;
        [SerializeField] private LayerMask whatIsEnemy;
        [SerializeField] private LayerMask whatIsGround;
        
        public int Damage => damage;
        public float Speed => speed;
        public float HitReturnDelay => hitReturnDelay;
        public float MissReturnDelay => missReturnDelay;
        public LayerMask WhatIsPlayer => whatIsPlayer;
        public LayerMask WhatIsEnemy => whatIsEnemy;
        public LayerMask WhatIsGround => whatIsGround;
    }
}