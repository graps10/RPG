using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Items_and_Inventory
{
    public enum ItemType
    {
        Material,
        Equipment
    }

    [CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item", order = 0)]
    public class ItemData : ScriptableObject
    {
        public ItemType ItemType;
        public string ItemName;
        public Sprite Icon;
        public string ItemID;

        [Range(0, 100)]
        public float DropChance;

        protected StringBuilder sb = new();

        private void OnValidate()
        {
#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(this);
            ItemID = AssetDatabase.AssetPathToGUID(path);
#endif
        }
        public virtual string GetDescription()
        {
            return "";
        }
    }
}