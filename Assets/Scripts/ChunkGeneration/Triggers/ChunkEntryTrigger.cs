using UnityEngine;

namespace ChunkGeneration.Triggers
{
    public class ChunkEntryTrigger : ChunkTrigger
    {
        public override void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<Player.Player>() != null)
                chunkController.OnPlayerEntry();
        }
    }
}
