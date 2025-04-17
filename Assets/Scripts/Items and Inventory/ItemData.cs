using System.Text;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public enum ItemType
{
    Material,
    Equipment
}

[CreateAssetMenu(fileName = "New Item Data", menuName = "Data/Item", order = 0)]
public class ItemData : ScriptableObject
{
    public ItemType itemType;
    public string itemName;
    public Sprite icon;
    public string itemID;

    [Range(0, 100)]
    public float dropChance;

    protected StringBuilder sb = new StringBuilder();

    void OnValidate()
    {
#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(this);
            itemID = AssetDatabase.AssetPathToGUID(path);
#endif
    }
    public virtual string GetDescription()
    {
        return "";
    }
}

