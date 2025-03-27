using UnityEngine;

public class LostCurrencyController : MonoBehaviour
{
    public int currency;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>() != null)
        {
            PlayerManager.instance.currency += currency;
            Destroy(this.gameObject);
        }   
    }
}
