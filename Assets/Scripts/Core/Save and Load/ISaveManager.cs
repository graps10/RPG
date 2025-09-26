namespace Core.Save_and_Load
{
    public interface ISaveManager
    {
        void LoadData(GameData data);
        void SaveData(ref GameData data);
    }
}
