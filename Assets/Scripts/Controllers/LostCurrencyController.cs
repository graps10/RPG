using Managers;
using UnityEngine;

namespace Controllers
{
    public class LostCurrencyController : MonoBehaviour
    {
        private int _currency;
        
        public void SetCurrency(int currency) => _currency = currency;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.GetComponent<Player.Player>() != null)
            {
                PlayerManager.Instance.AddCurrency(_currency);
                Destroy(gameObject);
            }   
        }
    }
}
