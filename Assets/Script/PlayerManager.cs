using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;

    public TMP_InputField nameInput;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void AddScore(int baseScore)
    {
        ComboManager.Instance.IncreaseCombo(); // Tambah combo jika menambah score
        ScoreManager.Instance.AddScore(baseScore);
    }

    public void SubmitScore()
    {
        string playerName = nameInput.text;
        ScoreManager.Instance.SaveScore(playerName);
        ComboManager.Instance.ResetCombo();
    }
}
