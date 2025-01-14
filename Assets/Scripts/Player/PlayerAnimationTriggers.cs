using UnityEngine;

public class PlayerAnimationTriggers : MonoBehaviour
{
    [SerializeField] private Player player;

    private void AnimatinTrigger()
    {
        player.AnimationTrigger();
    }
}
