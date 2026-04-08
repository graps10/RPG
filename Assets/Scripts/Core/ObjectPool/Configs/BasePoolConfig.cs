using UnityEngine;

namespace Core.ObjectPool.Configs
{
    [CreateAssetMenu(fileName = "BasePoolConfig", menuName = "ObjectPool/Configs/BasePoolConfig")]
    public class BasePoolConfig : ScriptableObject
    {
        [Header("Pool Settings")]
        [SerializeField] private PooledObject prefab;
        
        public GameObject Prefab => prefab.gameObject;
    }
}

