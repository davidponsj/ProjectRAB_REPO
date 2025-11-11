using UnityEngine;
using TMPro;

public class BestTimeDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bestTimeText;
    [SerializeField] int levelNumber = 1;

    void Start()
    {
        string key = "BestTime_Level" + levelNumber;
        float best = PlayerPrefs.GetFloat(key, 0f);

        if (best > 0)
        {
            int minutes = Mathf.FloorToInt(best / 60);
            int seconds = Mathf.FloorToInt(best % 60);
            bestTimeText.text = $"Mejor tiempo: {minutes:00}:{seconds:00}";
        }
        else
        {
            bestTimeText.text = "Aún no hay mejor tiempo";
        }
    }
}
