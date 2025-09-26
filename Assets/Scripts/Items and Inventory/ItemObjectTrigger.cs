using Stats;
using UnityEngine;

namespace Items_and_Inventory
{
    public class ItemObjectTrigger : MonoBehaviour
    {
        private ItemObject _myItemObject => GetComponentInParent<ItemObject>();

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.GetComponent<Player.Player>() != null)
            {
                if (collision.GetComponent<CharacterStats>().IsDead)
                    return;
                
                _myItemObject.PickupItem();
            }
        }
    }
}
