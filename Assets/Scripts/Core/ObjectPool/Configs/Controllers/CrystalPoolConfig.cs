using UnityEngine;

namespace Core.ObjectPool.Configs.Controllers
{
    [CreateAssetMenu(fileName = "CrystalPoolConfig", menuName = "ObjectPool/Configs/Controllers/Crystal")]
    public class CrystalPoolConfig : BasePoolConfig
    {
        [Header("Crystal Specific Settings")]
        [SerializeField] private float growSpeed = 5;
        [SerializeField] private Vector2 growTargetScale = new(3f, 3f);
        [SerializeField] private float closestTargetMinDistance = 1f;
        [SerializeField] private LayerMask whatIsEnemy;
        
        public float GrowSpeed => growSpeed;
        public Vector2 GrowTargetScale => growTargetScale;
        public float ClosestTargetMinDistance => closestTargetMinDistance;
        public LayerMask WhatIsEnemy => whatIsEnemy;
    }
}