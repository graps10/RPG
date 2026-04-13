using UnityEngine;

namespace Core.ObjectPool.Configs.Enemies
{
    [CreateAssetMenu(fileName = "ShadyPoolConfig", menuName = "ObjectPool/Configs/Enemies/Shady")]
    public class ShadyPoolConfig : EnemyPoolConfig
    {
        [Header("Shady Specific Settings")]
        [SerializeField] private float battleStateMoveSpeed;
        [SerializeField] private GameObject explosivePrefab;
        [SerializeField] private float growSpeed;
        [SerializeField] private float maxSize;
        
        public float BattleStateMoveSpeed =>  battleStateMoveSpeed;
        public GameObject ExplosivePrefab => explosivePrefab;
        public float GrowSpeed => growSpeed;
        public float MaxSize => maxSize;
    }
}