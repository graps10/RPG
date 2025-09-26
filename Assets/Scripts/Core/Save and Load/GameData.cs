using System.Collections.Generic;
using UnityEngine;

namespace Core.Save_and_Load
{
    [System.Serializable]
    public class GameData
    {
        [SerializeField] private int currency;

        [SerializeField] private int lostCurrencyAmount;
        [SerializeField] private Vector2 spawnLostCurrencyRange;
        
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
            SetSpawnLostCurrencyRange(Vector2.zero);

            skillTree = new SerializableDictionary<string, bool>();
            inventory = new SerializableDictionary<string, int>();
            equipmentIDs = new List<string>();

            /*closestCheckpointId = string.Empty;
            checkpoints = new SerializableDictionary<string, bool>();*/

            volumeSettings = new SerializableDictionary<string, float>();
        }
        
        public void SetCurrency(int currency) => this.currency = currency;
        
        public void SetLostCurrencyAmount(int amount) => lostCurrencyAmount = amount;
        public void SetSpawnLostCurrencyRange(Vector2 range) => spawnLostCurrencyRange = range;
        

        #region Game Data Getters
        
        public int GetCurrency() => currency;
        
        public int GetLostCurrencyAmount() => lostCurrencyAmount;
        public Vector2 GetSpawnLostCurrencyRange() => spawnLostCurrencyRange;
        
        public Dictionary<string, bool> GetSkillTree() => skillTree;
        
        public Dictionary<string, int> GetInventory() => inventory;
        
        public List<string> GetEquipmentList() => equipmentIDs;
        
        public Dictionary<string, float> GetVolumeSettings() => volumeSettings;
        
        #endregion
    }
}
