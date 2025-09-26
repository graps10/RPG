using UnityEngine;

namespace Items_and_Inventory.Effects
{
    public class ItemEffect : ScriptableObject
    {
        [TextArea]
        [SerializeField] private string effectDescription;
        
        public virtual void ExecuteEffect()
        {
            Debug.Log("Effect executed!");
        }
        
        public virtual void ExecuteEffect(Transform enemyPosition)
        {
            Debug.Log("Effect executed!");
        }
        
        public string GetEffectDescription() => effectDescription;
    }
}
