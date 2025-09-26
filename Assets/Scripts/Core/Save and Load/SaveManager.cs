using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Core.Save_and_Load
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance;

        [SerializeField] private string fileName;
        [SerializeField] private bool encryptData;

        private GameData _gameData;
        private List<ISaveManager> _saveManagers;
        private FileDataHandler _dataHandler;

        [ContextMenu("Delete save file")]
        public void DeleteSavedData()
        {
            _dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
            _dataHandler.Delete();
        }

        private void Awake()
        {
            if (Instance != null)
                Destroy(Instance.gameObject);
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            _dataHandler = new FileDataHandler(Application.persistentDataPath, fileName, encryptData);
            _saveManagers = FindAllSaveManagers();
            
            LoadGame();
        }

        public void NewGame()
        {
            _gameData = new GameData();
        }

        public void LoadGame()
        {
            _gameData = _dataHandler.Load();

            if (_gameData == null)
            {
                Debug.Log("No saved data found");
                NewGame();
            }

            foreach (ISaveManager saveManager in _saveManagers)
                saveManager.LoadData(_gameData);

            Debug.Log("Loaded currency " + _gameData.GetCurrency());
        }

        [ContextMenu("Save Game")]
        public void SaveGame()
        {
            foreach (ISaveManager saveManager in _saveManagers)
                saveManager.SaveData(ref _gameData);
            
            _dataHandler.Save(_gameData);
        }

        public bool HasSavedData()
        {
            if (_dataHandler.Load() != null)
                return true;

            return false;
        }

        private void OnApplicationQuit()
        {
            SaveGame();
        }

        private List<ISaveManager> FindAllSaveManagers()
        {
            IEnumerable<ISaveManager> saveManagers = FindObjectsOfType<MonoBehaviour>().OfType<ISaveManager>();
            return new List<ISaveManager>(saveManagers);
        }
    }
}
