using Managers;
using UnityEngine;

namespace Components.Audio
{
    public class AreaSound : MonoBehaviour
    {
        [SerializeField] private int areaSoundIndex;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<Player.Player>() != null)
                AudioManager.Instance.PlaySFX(areaSoundIndex, null);
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<Player.Player>() != null)
                AudioManager.Instance.StopSFXWithDelay(areaSoundIndex);
        }
    }
}
