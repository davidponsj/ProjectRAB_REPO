using UnityEngine;
using TMPro;

public class BestTimeMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bestTimeText;
    [SerializeField] private int levelNumber = 1; // Número del nivel que quieres mostrar

    void Start()
    {
        UpdateBestTime();
    }

    void UpdateBestTime()
    {
        string key = "BestTime_Level" + levelNumber;
        if (PlayerPrefs.HasKey(key))
        {
            float best = PlayerPrefs.GetFloat(key);
            int minutes = Mathf.FloorToInt(best / 60);
            int seconds = Mathf.FloorToInt(best % 60);
            bestTimeText.text = $"Mejor tiempo Nivel {levelNumber}: {minutes:00}:{seconds:00}";
        }
        else
        {
            bestTimeText.text = $"Aún no hay mejor tiempo Nivel {levelNumber}";
        }
    }
}