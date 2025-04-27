using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoolConfig", menuName = "Pooling/PoolConfig")]
public class PoolConfig : ScriptableObject
{
    public string key;
    public List<GameObject> prefabs = new List<GameObject>();
    public int initialSize = 10;
    public bool allowExpand = true;
}

