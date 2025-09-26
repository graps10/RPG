using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Save_and_Load
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue> , ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> keys = new();
        [SerializeField] private List<TValue> values = new();

        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach(KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();

            if(keys.Count != values.Count)
            {
                Debug.Log("Keys count is not equal to values count");
            }

            for (int i = 0; i < keys.Count; i++)
                Add(keys[i], values[i]);
        }
    }
}
