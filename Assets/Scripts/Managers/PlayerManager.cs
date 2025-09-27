using Core.Save_and_Load;
using Stats;
using UnityEngine;

namespace Managers
{
    public class PlayerManager : MonoBehaviour, ISaveManager
    {
        public static PlayerManager Instance;
        
        public Player.Player PlayerGameObject;
        public PlayerStats PlayerStats { get; private set; }

        private int _currency;

        private void Awake()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            else
                Instance = this;
            
            PlayerStats = PlayerGameObject.GetComponent<PlayerStats>();
        }

        public void SpawnPlayer(Transform spawnSpot)
        {
            PlayerGameObject.transform.position = spawnSpot.position;
            PlayerGameObject.gameObject.SetActive(true);
        }

        public bool HasEnoughMoney(int _price)
        {
            if (_price > _currency)
            {
                Debug.Log("Not enough money");
                return false;
            }

            _currency -= _price;
            return true;
        }

        public void AddCurrency(int amount) => _currency += amount;
        public void SetCurrency(int currency) => _currency = currency;
        public int GetCurrency() => _currency;

        public void LoadData(GameData data)
        {
            _currency = data.GetCurrency();
        }

        public void SaveData(ref GameData data)
        {
            data.SetCurrency(_currency);
        }
    }
}
