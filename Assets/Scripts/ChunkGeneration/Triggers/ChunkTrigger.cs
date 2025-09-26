using UnityEngine;

namespace ChunkGeneration.Triggers
{
    public class ChunkTrigger : MonoBehaviour
    {
        protected ChunkController chunkController;
        protected BoxCollider2D trigger => GetComponent<BoxCollider2D>();

        public virtual void Initialize(ChunkController controller)
        {
            chunkController = controller;
        }

        public virtual void OnTriggerExit2D(Collider2D collision) { }

        public virtual void EnableTrigger() => trigger.isTrigger = true;
        public virtual void DisableTrigger() => trigger.isTrigger = false;
    }
}
