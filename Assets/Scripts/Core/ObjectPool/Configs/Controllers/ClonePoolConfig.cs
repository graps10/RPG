using UnityEngine;

namespace Core.ObjectPool.Configs.Controllers
{
    [CreateAssetMenu(fileName = "ClonePoolConfig", menuName = "ObjectPool/Configs/Controllers/Clone")]
    public class ClonePoolConfig : BasePoolConfig
    {
        [Header("Clone Base Settings")]
        [SerializeField] private float colorLosingSpeed = 0.5f;
        [SerializeField] private float cloneDelay = 0.4f;
        [SerializeField] private Vector3 cloneOffset = new(2f, 0f, 0f);
        [SerializeField] private Vector3 cloneDuplicateOffset = new(0.5f, 0f);
        
        [Header("Animation & Detection")]
        [SerializeField] private Vector2 attackAnimRange = new(1, 4);
        [SerializeField] private float closestEnemyCheckRadius = 10f;
        [SerializeField] private LayerMask whatIsEnemy;
        
        public float ColorLosingSpeed => colorLosingSpeed;
        public float CloneDelay => cloneDelay;
        public Vector3 CloneOffset => cloneOffset;
        public Vector3 CloneDuplicateOffset => cloneDuplicateOffset;
        public Vector2 AttackAnimRange => attackAnimRange;
        public float ClosestEnemyCheckRadius => closestEnemyCheckRadius;
        public LayerMask WhatIsEnemy => whatIsEnemy;
    }
}