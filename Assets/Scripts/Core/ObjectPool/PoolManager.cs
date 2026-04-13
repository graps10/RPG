using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Core.ObjectPool
{
    public class PoolManager : MonoBehaviour
    {
        private const int Default_Capacity = 10;
        private const int Max_Size = 500;
        
        public static PoolManager Instance { get; private set; }
        
        private Dictionary<int, IObjectPool<GameObject>> _pools = new();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            int prefabId = prefab.GetInstanceID();
            
            if (!_pools.ContainsKey(prefabId))
                CreatePool(prefab, prefabId);
            
            GameObject obj = _pools[prefabId].Get();
            obj.transform.SetPositionAndRotation(position, rotation);
            
            return obj;
        }

        private void CreatePool(GameObject prefab, int prefabId)
        {
            Transform poolParent = new GameObject($"Pool_{prefab.name}").transform;
            poolParent.SetParent(transform);

            var pool = new ObjectPool<GameObject>(
                createFunc: () =>
                {
                    GameObject obj = Instantiate(prefab, poolParent);
                    
                    if (!obj.TryGetComponent(out PooledObject pooledObj))
                        pooledObj = obj.AddComponent<PooledObject>();
                    
                    pooledObj.SetPool(_pools[prefabId]); 
                    return obj;
                },
                actionOnGet: obj => obj.SetActive(true),
                actionOnRelease: obj => obj.SetActive(false),
                actionOnDestroy: obj => Destroy(obj),
                defaultCapacity: Default_Capacity,
                maxSize: Max_Size
            );
            
            _pools.Add(prefabId, pool);
        }
        
        public void Return(GameObject obj)
        {
            if (obj.TryGetComponent(out PooledObject pooledObj))
                pooledObj.ReturnToPool();
            else
                obj.SetActive(false);
        }
        
        public void ReturnWithDelay(GameObject obj, float delay)
        {
            StartCoroutine(ReturnCoroutine(obj, delay));
        }

        private IEnumerator ReturnCoroutine(GameObject obj, float delay)
        {
            yield return new WaitForSeconds(delay);
            
            if (obj == null) yield break; 
            Return(obj);
        }
    }
}