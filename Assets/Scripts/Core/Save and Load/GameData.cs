using System.Collections.Generic;
using UnityEngine;

namespace Core.Save_and_Load
{
    [System.Serializable]
    public class GameData
    {
        [SerializeField] private int currency;

        [SerializeField] private int lostCurrencyAmount;
        [SerializeField] private bool showPlayerHealthBar;
        //[SerializeField] private Vector2 spawnLostCurrencyRange;

        // Options / Settings
        [SerializeField] private bool fullscreen;
        [SerializeField] private int resolutionIndex;
        [SerializeField] private int graphicsQuality;
        [SerializeField] private bool showDamageNumbers;
        [SerializeField] private bool cameraShake;
        
        [SerializeField] private SerializableDictionary<string, bool> skillTree;
        [SerializeField] private SerializableDictionary<string, int> inventory;
        [SerializeField] private List<string> equipmentIDs;

        /*public SerializableDictionary<string, bool> checkpoints;
        public string closestCheckpointId;*/
        
        [SerializeField] private SerializableDictionary<string, float> volumeSettings;

        public GameData()
        {
            SetCurrency(0);
            SetLostCurrencyAmount(0);
            showPlayerHealthBar = true;
            //SetSpawnLostCurrencyRange(Vector2.zero);

            fullscreen = true;
            resolutionIndex = -1; // -1 = keep current system resolution on first load
            graphicsQuality = 2;  // safe middle preset; actual level applied from QualitySettings on load
            showDamageNumbers = true;
            cameraShake = true;

            skillTree = new SerializableDictionary<string, bool>();
            inventory = new SerializableDictionary<string, int>();
            equipmentIDs = new List<string>();

            /*closestCheckpointId = string.Empty;
            checkpoints = new SerializableDictionary<string, bool>();*/

            volumeSettings = new SerializableDictionary<string, float>();
        }
        
        public void SetCurrency(int currency) => this.currency = currency;
        
        public void SetLostCurrencyAmount(int amount) => lostCurrencyAmount = amount;
        //public void SetSpawnLostCurrencyRange(Vector2 range) => spawnLostCurrencyRange = range;
        
        public void SetShowPlayerHealthBar(bool value) => showPlayerHealthBar = value;

        // Options setters
        public void SetFullscreen(bool value) => fullscreen = value;
        public void SetResolutionIndex(int value) => resolutionIndex = value;
        public void SetGraphicsQuality(int value) => graphicsQuality = value;
        public void SetShowDamageNumbers(bool value) => showDamageNumbers = value;
        public void SetCameraShake(bool value) => cameraShake = value;

        #region Game Data Getters

        public int GetCurrency() => currency;

        public int GetLostCurrencyAmount() => lostCurrencyAmount;
        //public Vector2 GetSpawnLostCurrencyRange() => spawnLostCurrencyRange;

        public bool ShowPlayerHealthBar => showPlayerHealthBar;

        // Options getters
        public bool GetFullscreen() => fullscreen;
        public int GetResolutionIndex() => resolutionIndex;
        public int GetGraphicsQuality() => graphicsQuality;
        public bool GetShowDamageNumbers() => showDamageNumbers;
        public bool GetCameraShake() => cameraShake;

        public Dictionary<string, bool> GetSkillTree() => skillTree;

        public Dictionary<string, int> GetInventory() => inventory;

        public List<string> GetEquipmentList() => equipmentIDs;

        public Dictionary<string, float> GetVolumeSettings() => volumeSettings;

        #endregion
    }
}
