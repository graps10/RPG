using Controllers;
using Core.Save_and_Load;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Managers
{
    public class GameManager : MonoBehaviour, ISaveManager
    {
        public static GameManager Instance;

        // public string closestCheckpointId;
        // [SerializeField] private Checkpoint[] checkpoints;

        [Header("Lost Currency")]
        [SerializeField] private GameObject lostCurrencyPrefab;
        [SerializeField] private Vector2 spawnLostCurrencyRange;
    
        private int _lostCurrencyAmount;

        private void Awake()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            else
                Instance = this;
        
            // temporary find checkpoints
        }
    
        public void SetLostCurrencyAmount(int amount) => _lostCurrencyAmount = amount;
        public int GetLostCurrencyAmount() => _lostCurrencyAmount;

        public void RestartScene()
        {
            SaveManager.Instance.SaveGame();
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }

        #region Load

        public void LoadData(GameData data)
        {
            LoadLostCurrency(data);
            // LoadCheckpoints(_data);
        }

        private void LoadLostCurrency(GameData data)
        {
            _lostCurrencyAmount = data.GetLostCurrencyAmount();
            spawnLostCurrencyRange = data.GetSpawnLostCurrencyRange();

            if (_lostCurrencyAmount > 0)
            {
                GameObject newLostCurrency = Instantiate(lostCurrencyPrefab, 
                    new Vector3(spawnLostCurrencyRange.x, spawnLostCurrencyRange.y), Quaternion.identity);
            
                newLostCurrency.GetComponent<LostCurrencyController>().SetCurrency(_lostCurrencyAmount);;
            }

            _lostCurrencyAmount = 0;
        }

        #endregion

        #region Save

        public void SaveData(ref GameData data)
        {
            SaveLostCurrency(data);
        }

        private void SaveLostCurrency(GameData data)
        {
            data.SetLostCurrencyAmount(_lostCurrencyAmount);

            var playerPos = PlayerManager.Instance.PlayerGameObject.transform.position;
            data.SetSpawnLostCurrencyRange(new Vector2(playerPos.x, playerPos.y));
        }

        #endregion

        public void PauseGame(bool _pause)
        {
            Time.timeScale = _pause ? 0 : 1;
        }
    }
}
