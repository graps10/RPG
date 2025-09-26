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

        public GameObject Spawn(string _key, Vector3 _position, Quaternion _rotation, GameObject _specificPrefab = null)
        {
            if (_pools.TryGetValue(_key, out var pool))
            {
                return pool.Spawn(_position, _rotation, _specificPrefab);
            }
            Debug.LogWarning($"Pool with key '{_key}' not found.");
            return null;
        }

        public void Return(string _key, GameObject _obj)
        {
            if (_pools.TryGetValue(_key, out var pool))
            {
                pool.Return(_obj);
            }
            else
            {
                Debug.LogWarning($"Return failed. Pool with key '{_key}' not found.");
                Destroy(_obj);
            }
        }

        public void Return(string _key, GameObject _obj, float _delay) 
            => StartCoroutine(ReturnToPoolRoutine(_key, _obj, _delay));

        private IEnumerator ReturnToPoolRoutine(string _key, GameObject _obj, float _delay)
        {
            yield return new WaitForSeconds(_delay);
            Return(_key, _obj);
        }
    }
}
