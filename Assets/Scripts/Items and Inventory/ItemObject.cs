using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private ItemData itemData;
    [SerializeField] private Vector2 velocity;

    // void Update() {
    //     if(Input.GetKeyDown(KeyCode.M))
    //     {
    //         rb.velocity = velocity;
    //     }
    // }

    public void SetupItem(ItemData _itemData, Vector2 _velocity)
    {

        itemData = _itemData;
        rb.velocity = _velocity;

        SetupVisual();
    }

    public void PickupItem()
    {
        if (!Inventory.instance.CanAddItem() && itemData.itemType == ItemType.Equipment)
        {
            rb.velocity = new Vector2(0, 7);
            PlayerManager.instance.player.fx.CreatePopUpText("Inventory is full");
            return;
        }
        AudioManager.instance.PlaySFX(16, transform);
        Inventory.instance.AddItem(itemData);
        Destroy(gameObject);
    }

    private void SetupVisual()
    {
        if (itemData == null) return;

        GetComponent<SpriteRenderer>().sprite = itemData.icon;
        gameObject.name = "Item object - " + itemData.itemName;
    }
}
