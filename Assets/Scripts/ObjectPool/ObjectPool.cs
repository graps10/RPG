using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    private Queue<GameObject> poolQueue = new();
    private PoolConfig config;
    private Transform poolParent;

    public ObjectPool(PoolConfig config, Transform parent = null)
    {
        this.config = config;
        this.poolParent = parent;

        for (int i = 0; i < config.initialSize; i++)
        {
            var obj = CreateNewObject();
            poolQueue.Enqueue(obj);
        }
    }

    private GameObject CreateNewObject()
    {
        var obj = Object.Instantiate(config.prefab);
        obj.SetActive(false);
        obj.transform.SetParent(poolParent);
        return obj;
    }

    public GameObject Spawn(Vector3 position, Quaternion rotation)
    {
        GameObject obj = poolQueue.Count > 0 ? poolQueue.Dequeue() :
                        (config.allowExpand ? CreateNewObject() : null);

        if (obj == null) return null;

        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        if (obj.TryGetComponent<PooledObject>(out var pooled))
        {
            pooled.OnSpawn();
        }

        return obj;
    }

    public void Return(GameObject obj)
    {
        if (obj.TryGetComponent<PooledObject>(out var pooled))
        {
            pooled.OnReturnToPool();
        }

        obj.transform.SetParent(poolParent);
        obj.SetActive(false);
        poolQueue.Enqueue(obj);
    }
}
