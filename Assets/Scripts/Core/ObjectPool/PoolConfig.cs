using System.Collections.Generic;
using UnityEngine;

namespace Core.ObjectPool
{
    [CreateAssetMenu(fileName = "PoolConfig", menuName = "Pooling/PoolConfig")]
    public class PoolConfig : ScriptableObject
    {
        public string Key;
        public List<GameObject> Prefabs = new();
        public int InitialSize = 10;
        public bool AllowExpand = true;
    }
}

