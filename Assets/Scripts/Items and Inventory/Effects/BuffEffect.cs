using Managers;
using Stats;
using UnityEngine;

namespace Items_and_Inventory.Effects
{
    [CreateAssetMenu(fileName = "Buff effect", menuName = "Data/Item Effect/Buff effect", order = 0)]
    public class BuffEffect : ItemEffect
    {
        [SerializeField] private StatType buffType;
        [SerializeField] private int buffAmount;
        [SerializeField] private float buffDuration;
        
        private PlayerStats _stats;

        public override void ExecuteEffect()
        {
            _stats = PlayerManager.Instance.PlayerGameObject.GetComponent<PlayerStats>();
            _stats.IncreaseStatBy(buffAmount, buffDuration, _stats.GetStat(buffType));
        }
    }
}
