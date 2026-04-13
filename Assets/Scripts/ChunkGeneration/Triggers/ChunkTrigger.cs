using UnityEngine;

namespace ChunkGeneration.Triggers
{
    public class ChunkTrigger : MonoBehaviour
    {
        [SerializeField] private GameObject solidWall;
        
        protected ChunkController chunkController;
        protected BoxCollider2D trigger => GetComponent<BoxCollider2D>();

        public virtual void Initialize(ChunkController controller) => chunkController = controller;

        public virtual void EnablePhysicalWall()
        {
            if (solidWall != null) 
                solidWall.SetActive(true);
        }

        public virtual void DisablePhysicalWall()
        {
            if (solidWall != null) 
                solidWall.SetActive(false);
        }
    }
}
