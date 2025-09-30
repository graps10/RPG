using Cinemachine;
using Core.Save_and_Load;
using UnityEngine;

namespace Managers
{
    public class PlayerManager : MonoBehaviour, ISaveManager
    {
        public static PlayerManager Instance;
        
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private CinemachineVirtualCamera playerCamera;
        
        public Player.Player PlayerGameObject { get; private set; }
        
        private GameObject _createdPlayer;
        private Transform _defaultSpawnPoint;
        
        private int _currency;

        private void Awake()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            else
                Instance = this;
            
            ValidatePlayerPrefab();
            SpawnPlayer(Vector2.zero);
        }
        
        private void ValidatePlayerPrefab()
        {
            if (playerPrefab == null)
            {
                Debug.LogError("PlayerManager: Player prefab not assigned!");
                return;
            }
            
            if (playerPrefab.GetComponent<Player.Player>() == null)
            {
                Debug.LogError("PlayerManager: Player prefab missing Player component!");
            }
        }

        #region Spawning

        private void SpawnPlayer(Vector2 spawnPoint)
        {
            if (_createdPlayer != null)
            {
                Debug.Log("Destroying existing player instance");
                Destroy(_createdPlayer);
            }

            _createdPlayer = Instantiate(playerPrefab, spawnPoint, Quaternion.identity);
            PlayerGameObject = _createdPlayer.GetComponent<Player.Player>();
            playerCamera.Follow = PlayerGameObject.transform;
            
            PlayerGameObject.gameObject.name = "Player";
            if(!PlayerGameObject.gameObject.activeInHierarchy)
                PlayerGameObject.gameObject.SetActive(true);
        }
        
        public void TeleportPlayer(Vector2 teleportSpot)
        {
            if (PlayerGameObject == null)
            {
                Debug.LogWarning("No player found, spawning new one...");
                SpawnPlayer(teleportSpot);
                return;
            }
            
            PlayerGameObject.transform.position = teleportSpot;
            
            if(!PlayerGameObject.gameObject.activeInHierarchy)
                PlayerGameObject.gameObject.SetActive(true);
        }

        #endregion
        
        #region Currency Management
        
        public bool HasEnoughMoney(int price)
        {
            if (price > _currency)
            {
                Debug.Log("Not enough money");
                return false;
            }

            _currency -= price;
            return true;
        }

        public void AddCurrency(int amount) => _currency += amount;
        public void SetCurrency(int currency) => _currency = currency;
        public int GetCurrency() => _currency;
        
        #endregion

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
