using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

[System.Serializable]
public class PlayerData
{
    public string playerName;
    public int score;
}

[System.Serializable]
public class PlayerDataList
{
    public List<PlayerData> players = new List<PlayerData>();
}

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    public int totalScore;
    public TMP_Text scoreDisplay;
    public TMP_Text BestLeaderBoardName;
    public TMP_Text BestLeaderBoardScore;

    private PlayerDataList playerDataList = new PlayerDataList();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    void Start()
    {
        LoadData();
        UpdateAllScoresUI();
    }

    public void AddScore(int baseScore)
    {
        int finalScore = baseScore * ComboManager.Instance.GetComboMultiplier();
        totalScore += finalScore;
        scoreDisplay.text = $"Score: {totalScore}";
    }

    public void SaveScore(string playerName)
    {
        if (string.IsNullOrEmpty(playerName))
        {
            Debug.Log("Nama tidak boleh kosong!");
            return;
        }

        playerDataList.players.Add(new PlayerData { playerName = playerName, score = totalScore });

        SaveData();
        totalScore = 0;
        scoreDisplay.text = "Score: " + totalScore;
        UpdateAllScoresUI();
    }

    private void SaveData()
    {
        string json = JsonUtility.ToJson(playerDataList);
        PlayerPrefs.SetString("PlayerScores", json);
        PlayerPrefs.Save();
    }

    public void LoadData()
    {
        string json = PlayerPrefs.GetString("PlayerScores", "{}");
        playerDataList = JsonUtility.FromJson<PlayerDataList>(json);
    }

    private void UpdateAllScoresUI()
    {
        BestLeaderBoardName.text = "";
        BestLeaderBoardScore.text = "";

        var sortedPlayers = playerDataList.players.OrderByDescending(p => p.score).ToList();

        foreach (var player in sortedPlayers)
        {
            BestLeaderBoardName.text += $"{player.playerName}\n";
            BestLeaderBoardScore.text += $"{player.score}\n";
        }
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteKey("PlayerScores");
        playerDataList.players.Clear();
        PlayerPrefs.Save();
        UpdateAllScoresUI();
        Debug.Log("Semua data telah dihapus!");
    }
}
