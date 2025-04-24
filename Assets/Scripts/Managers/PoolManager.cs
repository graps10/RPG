using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public static PoolManager instance { get; private set; }

    [SerializeField] private List<PoolConfig> poolConfigs;
    private Dictionary<string, ObjectPool> pools = new();
    private Dictionary<string, Transform> poolParents = new();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var config in poolConfigs)
        {
            var poolParent = new GameObject($"{config.key}_Pool");
            poolParent.transform.SetParent(transform);
            poolParents.Add(config.key, poolParent.transform);

            var pool = new ObjectPool(config, poolParent.transform);
            pools.Add(config.key, pool);
        }
    }

    public GameObject Spawn(string key, Vector3 position, Quaternion rotation)
    {
        if (pools.TryGetValue(key, out var pool))
        {
            return pool.Spawn(position, rotation);
        }
        Debug.LogWarning($"Pool with key '{key}' not found.");
        return null;
    }

    public void Return(string key, GameObject obj)
    {
        if (pools.TryGetValue(key, out var pool))
        {
            pool.Return(obj);
        }
        else
        {
            Debug.LogWarning($"Return failed. Pool with key '{key}' not found.");
            Destroy(obj);
        }
    }
}
