using UnityEngine;

public class PlayerManager : MonoBehaviour, ISaveManager
{
    public static PlayerManager instance;
    public Player player;

    public int currency;

    void Awake()
    {
        if(instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;
    }

    public bool HasEnoughMoney(int _price)
    {
        if(_price > currency)
        {
            Debug.Log("Not enough money");
            return false;
        }

        currency -= _price;
        return true;
    }

    public int GetCurrency() => currency;

    public void LoadData(GameData _data)
    {
        this.currency = _data.currency;
    }

    public void SaveData(ref GameData _data)
    {
        _data.currency = this.currency;
    }
}
