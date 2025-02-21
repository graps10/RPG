
using UnityEngine;

[CreateAssetMenu(fileName = "Buff effect", menuName = "Data/Item Effect/Buff effect", order = 0)]
public class Buff_Effect : ItemEffect
{
    private PlayerStats stats;
    [SerializeField] private StatType buffType;
    [SerializeField] private int buffAmount;
    [SerializeField] private float buffDuration;

    public override void ExucuteEffect(Transform _enemyPosition)
    {
        stats = PlayerManager.instance.player.GetComponent<PlayerStats>();

        stats.IncreaseStatBy(buffAmount, buffDuration, stats.GetStat(buffType));
    }
}
