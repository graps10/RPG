using System.Collections;
using System.Collections.Generic;
using Core.ObjectPool;
using UnityEngine;

namespace Managers
{
    public class PoolManager : MonoBehaviour
    {
        public static PoolManager Instance { get; private set; }

        [SerializeField] private List<PoolConfig> poolConfigs;
        
        private Dictionary<string, ObjectPool> _pools = new();
        private Dictionary<string, Transform> _poolParents = new();

        private void Awake()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            else
                Instance = this;

            InitializePools();
        }

        private void InitializePools()
        {
            foreach (var config in poolConfigs)
            {
                var poolParent = new GameObject($"{config.Key}_Pool");
                poolParent.transform.SetParent(transform);
                _poolParents.Add(config.Key, poolParent.transform);

                var pool = new ObjectPool(config, poolParent.transform);
                _pools.Add(config.Key, pool);
            }
        }

        public GameObject Spawn(string key, Vector3 position, Quaternion rotation, GameObject specificPrefab = null)
        {
            if (_pools.TryGetValue(key, out var pool))
            {
                return pool.Spawn(position, rotation, specificPrefab);
            }
            Debug.LogWarning($"Pool with key '{key}' not found.");
            return null;
        }

        public void Return(string key, GameObject obj)
        {
            if (_pools.TryGetValue(key, out var pool))
            {
                pool.Return(obj);
            }
            else
            {
                Debug.LogWarning($"Return failed. Pool with key '{key}' not found.");
                Destroy(obj);
            }
        }

        public void Return(string key, GameObject obj, float delay) 
            => StartCoroutine(ReturnToPoolRoutine(key, obj, delay));

        private IEnumerator ReturnToPoolRoutine(string key, GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            Return(key, obj);
        }
    }
}
