using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.ObjectPool
{
    public class ObjectPool
    {
        private PoolConfig _config;
        private Transform _poolParent;
        private Dictionary<GameObject, Queue<GameObject>> _prefabSpecificPools = new();

        public ObjectPool(PoolConfig config, Transform parent = null)
        {
            _config = config;
            _poolParent = parent;

            foreach (var prefab in _config.Prefabs)
            {
                _prefabSpecificPools[prefab] = new Queue<GameObject>();
                for (int i = 0; i < _config.InitialSize; i++)
                {
                    var obj = CreateNewObject(prefab);
                    _prefabSpecificPools[prefab].Enqueue(obj);
                }
            }
        }

        private GameObject CreateNewObject(GameObject prefab)
        {
            var obj = Object.Instantiate(prefab, _poolParent, true);
            obj.SetActive(false);
            return obj;
        }

        public GameObject Spawn(Vector3 position, Quaternion rotation, GameObject specificPrefab = null)
        {
            GameObject prefabToUse;

            if (specificPrefab != null && _config.Prefabs.Contains(specificPrefab))
                prefabToUse = specificPrefab;
            else
                prefabToUse = _config.Prefabs[Random.Range(0, _config.Prefabs.Count)];


            Queue<GameObject> specificQueue = _prefabSpecificPools[prefabToUse];
            GameObject obj = specificQueue.Count > 0 ? specificQueue.Dequeue() :
                (_config.AllowExpand ? CreateNewObject(prefabToUse) : null);

            if (obj == null) return null;

            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);

            if (obj.TryGetComponent<ISpawnedPooledObject>(out var pooled))
            {
                pooled.OnSpawn();
            }

            return obj;
        }

        public void Return(GameObject obj)
        {
            if (obj == null) return;

            if (obj.TryGetComponent<IPooledObject>(out var pooled))
            {
                pooled.OnReturnToPool();
            }

            GameObject originalPrefab = _config.Prefabs.FirstOrDefault(
                p => p.name.Replace("(Clone)", "") == obj.name.Replace("(Clone)", ""));

            if (originalPrefab != null)
            {
                obj.transform.SetParent(_poolParent);
                obj.SetActive(false);
                _prefabSpecificPools[originalPrefab].Enqueue(obj);
            }
        }
    }
}
