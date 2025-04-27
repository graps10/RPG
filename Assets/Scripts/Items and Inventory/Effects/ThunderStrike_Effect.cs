
using UnityEngine;

[CreateAssetMenu(fileName = "Thunder strike effect", menuName = "Data/Item Effect/Thunder strike", order = 0)]
public class ThunderStrike_Effect : ItemEffect
{
    [SerializeField] private GameObject thunderStrikePrefab;
    public override void ExucuteEffect(Transform _enemyPosition)
    {
        GameObject newThunderStrike = PoolManager.instance.Spawn("fx", _enemyPosition.position, Quaternion.identity, thunderStrikePrefab);
        PoolManager.instance.Return("fx", newThunderStrike, 1f);
    }
}
