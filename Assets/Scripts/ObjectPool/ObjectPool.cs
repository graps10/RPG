using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ObjectPool
{
    private PoolConfig config;
    private Transform poolParent;
    private Dictionary<GameObject, Queue<GameObject>> prefabSpecificPools = new();

    public ObjectPool(PoolConfig _config, Transform _parent = null)
    {
        config = _config;
        poolParent = _parent;

        foreach (var prefab in _config.prefabs)
        {
            prefabSpecificPools[prefab] = new Queue<GameObject>();
            for (int i = 0; i < _config.initialSize; i++)
            {
                var obj = CreateNewObject(prefab);
                prefabSpecificPools[prefab].Enqueue(obj);
            }
        }
    }

    private GameObject CreateNewObject(GameObject _prefab)
    {
        var obj = Object.Instantiate(_prefab);
        obj.SetActive(false);
        obj.transform.SetParent(poolParent);
        return obj;
    }

    public GameObject Spawn(Vector3 _position, Quaternion _rotation, GameObject _specificPrefab = null)
    {
        GameObject prefabToUse;

        if (_specificPrefab != null && config.prefabs.Contains(_specificPrefab))
            prefabToUse = _specificPrefab;
        else
            prefabToUse = config.prefabs[Random.Range(0, config.prefabs.Count)];


        Queue<GameObject> specificQueue = prefabSpecificPools[prefabToUse];
        GameObject obj = specificQueue.Count > 0 ? specificQueue.Dequeue() :
                        (config.allowExpand ? CreateNewObject(prefabToUse) : null);

        if (obj == null) return null;

        obj.transform.position = _position;
        obj.transform.rotation = _rotation;
        obj.SetActive(true);

        if (obj.TryGetComponent<IPooledObject>(out var pooled))
        {
            pooled.OnSpawn();
        }

        return obj;
    }

    public void Return(GameObject _obj)
    {
        if (_obj == null) return;

        if (_obj.TryGetComponent<IReturnedObject>(out var pooled))
        {
            pooled.OnReturnToPool();
        }

        GameObject originalPrefab = config.prefabs.FirstOrDefault(
            p => p.name.Replace("(Clone)", "") == _obj.name.Replace("(Clone)", ""));

        if (originalPrefab != null)
        {
            _obj.transform.SetParent(poolParent);
            _obj.SetActive(false);
            prefabSpecificPools[originalPrefab].Enqueue(_obj);
        }
    }
}
