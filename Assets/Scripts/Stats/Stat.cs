using System.Collections.Generic;
using UnityEngine;

namespace Stats
{
    [System.Serializable]
    public class Stat 
    {
        [SerializeField] private int baseValue;

        [SerializeField] private List<int> modifiers = new();

        public int GetValue()
        {
            int finalValue = baseValue;

            foreach(int modifier in modifiers)
                finalValue += modifier;

            return finalValue;
        }

        public void SetDefaultValue(int value)
        {
            baseValue = value;
        }

        public void AddModifier(int modifier)
        {
            modifiers.Add(modifier);
        }
    
        public void RemoveModifier(int modifier)
        {
            modifiers.Remove(modifier);
        }
    }
}
