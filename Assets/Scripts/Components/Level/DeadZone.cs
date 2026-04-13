using Core.ObjectPool;
using Stats;
using UnityEngine;

namespace Components.Level
{
    public class DeadZone : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<CharacterStats>() != null)
                collision.GetComponent<CharacterStats>().KillEntity();
            else
                PoolManager.Instance.Return(collision.gameObject);
        }
    }
}
