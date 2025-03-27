using System.Collections.Generic;

[System.Serializable]
public class GameData
{
    public int currency;

    public SerializableDictionary<string, bool> skillTree;
    public SerializableDictionary<string, int> inventory;
    public List<string> equipmentID;

    public SerializableDictionary<string, bool> checkpoints;
    public string closestCheckpointId;

    public int lostCurrencyAmount;
    public float lostCurrencyX;
    public float lostCurrencyY;

    public SerializableDictionary<string, float> volumeSettings;

    public GameData()
    {
        this.currency = 0;

        this.lostCurrencyAmount = 0;
        this.lostCurrencyX = 0;
        this.lostCurrencyY = 0;

        skillTree = new SerializableDictionary<string, bool>();
        inventory = new SerializableDictionary<string, int>();
        equipmentID = new List<string>();

        closestCheckpointId = string.Empty;
        checkpoints = new SerializableDictionary<string, bool>();

        volumeSettings = new SerializableDictionary<string, float>();
    }
}
