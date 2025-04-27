using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ISaveManager
{
    public static GameManager instance;
    private Transform player => PlayerManager.instance.player.transform;

    // public string closestCheckpointId;
    // [SerializeField] private Checkpoint[] checkpoints;

    [Header("Lost Currency")]
    [SerializeField] private GameObject lostCurrencyPrefab;
    public int lostCurrencyAmount;
    [SerializeField] private float lostCurrencyX;
    [SerializeField] private float lostCurrencyY;

    void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        else
            instance = this;

        // checkpoints = FindObjectsOfType<Checkpoint>();
    }

    public void RestartScene()
    {
        SaveManager.instance.SaveGame();
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    #region Load

    public void LoadData(GameData _data)
    {
        LoadLostCurrency(_data);
        // LoadCheckpoints(_data);
    }

    private void LoadLostCurrency(GameData _data)
    {
        lostCurrencyAmount = _data.lostCurrencyAmount;
        lostCurrencyX = _data.lostCurrencyX;
        lostCurrencyY = _data.lostCurrencyY;

        if (lostCurrencyAmount > 0)
        {
            GameObject newLostCurrency = Instantiate(lostCurrencyPrefab, new Vector3(lostCurrencyX, lostCurrencyY), Quaternion.identity);
            newLostCurrency.GetComponent<LostCurrencyController>().currency = lostCurrencyAmount;
        }

        lostCurrencyAmount = 0;
    }

    // private void LoadCheckpoints(GameData _data)
    // {
    //     foreach (KeyValuePair<string, bool> pair in _data.checkpoints)
    //     {
    //         foreach (Checkpoint checkpoint in checkpoints)
    //         {
    //             if (checkpoint.id == pair.Key && pair.Value == true)
    //                 checkpoint.ActivateCheckpoint();
    //         }
    //     }

    //     // closestCheckpointId = _data.closestCheckpointId;

    //     // Invoke("PlacePlayerAtClosestCheckpoint", 0.1f);
    //     PlacePlayerAtClosestCheckpoint(_data);
    // }

    #endregion

    #region Save

    public void SaveData(ref GameData _data)
    {
        SaveLostCurrency(_data);
        // SaveCheckpoints(_data);
    }

    private void SaveLostCurrency(GameData _data)
    {
        _data.lostCurrencyAmount = lostCurrencyAmount;
        _data.lostCurrencyX = player.position.x;
        _data.lostCurrencyY = player.position.y;
        // Debug.Log(player.position.x);
    }

    // private void SaveCheckpoints(GameData _data)
    // {
    //     if (FindClosestCheckPoint() == null) return;
    //     _data.closestCheckpointId = FindClosestCheckPoint().id;

    //     _data.checkpoints.Clear();

    //     foreach (Checkpoint checkpoint in checkpoints)
    //     {
    //         _data.checkpoints.Add(checkpoint.id, checkpoint.activationStatus);
    //     }
    // }

    #endregion

    public void PauseGame(bool _pause)
    {
        if (_pause)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
    // private void PlacePlayerAtClosestCheckpoint(GameData _data)
    // {
    //     if (_data.closestCheckpointId == null) return;

    //     foreach (Checkpoint checkpoint in checkpoints)
    //     {
    //         if (_data.closestCheckpointId == checkpoint.id)
    //             player.position = checkpoint.transform.position;
    //     }
    // }

    // private Checkpoint FindClosestCheckPoint()
    // {
    //     float closestDistance = Mathf.Infinity;
    //     Checkpoint closestCheckpoint = null;

    //     foreach (Checkpoint checkpoint in checkpoints)
    //     {
    //         float distanceToCheckpoint = Vector2.Distance(player.position, checkpoint.transform.position);
    //         if (distanceToCheckpoint < closestDistance && checkpoint.activationStatus == true)
    //         {
    //             closestDistance = distanceToCheckpoint;
    //             closestCheckpoint = checkpoint;
    //         }
    //     }

    //     return closestCheckpoint;
    // }
}
