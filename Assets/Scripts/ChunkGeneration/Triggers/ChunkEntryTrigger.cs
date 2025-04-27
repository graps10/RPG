using UnityEngine;

public class ChunkEntryTrigger : ChunkTrigger
{
    public override void Initialize(ChunkController controller)
    {
        base.Initialize(controller);
    }

    public override void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            chunkController.OnPlayerEntry();
        }
    }

    public override void EnableTrigger() => base.EnableTrigger();
    public override void DisableTrigger() => base.DisableTrigger();
}
