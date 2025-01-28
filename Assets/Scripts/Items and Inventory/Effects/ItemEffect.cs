using UnityEngine;

public class ItemEffect : ScriptableObject
{
    public virtual void ExucuteEffect(Transform _enemyPosition)
    {
        Debug.Log("Effect executed!");
    }
}
