using UnityEngine;

public class SkeletonAnimationTriggers : MonoBehaviour 
{
    [SerializeField] private Enemy_Skeleton enemy;

    private void AnimationTrigger()
    {
        enemy.AnimationTrigger();
    }
}