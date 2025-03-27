using UnityEngine;

public class AreaSound : MonoBehaviour
{
    [SerializeField] private int areaSoundIndex;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
            AudioManager.instance.PlaySFX(areaSoundIndex, null);
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
            AudioManager.instance.StopSFXWithDelay(areaSoundIndex);
    }
}
