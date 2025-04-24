using UnityEngine;

[CreateAssetMenu(fileName = "PoolConfig", menuName = "Pooling/PoolConfig")]
public class PoolConfig : ScriptableObject
{
    public string key;
    public GameObject prefab;
    public int initialSize = 10;
    public bool allowExpand = true;
}

