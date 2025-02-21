using UnityEngine;

public class ItemEffect : ScriptableObject
{
    [TextArea]
    public string effectDescription;
    public virtual void ExucuteEffect(Transform _enemyPosition)
    {
        Debug.Log("Effect executed!");
    }
}
