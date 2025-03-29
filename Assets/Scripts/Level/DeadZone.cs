using UnityEngine;

public class DeadZone : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<CharacterStats>() != null)
            collision.GetComponent<CharacterStats>().KillEntity();
        else
            Destroy(collision.gameObject);
    }
}
