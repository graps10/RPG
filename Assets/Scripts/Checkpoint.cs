using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    private Animator anim;
    public string id;
    public bool activationStatus;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            ActivateCheckpoint();
        }
    }

    public void ActivateCheckpoint()
    {
        if (!activationStatus)
            AudioManager.instance.PlaySFX(3, transform);

        activationStatus = true;
        anim.SetBool("active", true);
    }

    [ContextMenu("Generate checkpoint id")]
    private void GenerateID()
    {
        id = System.Guid.NewGuid().ToString();
    }
}
