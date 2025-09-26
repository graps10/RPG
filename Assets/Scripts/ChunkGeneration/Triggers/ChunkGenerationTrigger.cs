using UnityEngine;

namespace ChunkGeneration.Triggers
{
    public class ChunkGenerationTrigger : ChunkTrigger
    {
        public override void Initialize(ChunkController controller)
        {
            base.Initialize(controller);
            trigger.enabled = true;
        }

        public override void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<Player.Player>() != null)
            {
                chunkController.OnPlayerExitedLastChunk();
                trigger.enabled = false;
            }
        }
    }
}
