using UnityEngine;
using TMPro;
using System;

public class BestTimeDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI bestTimeText;
    [SerializeField] int levelNumber = 1;

    void OnEnable()
    {
        Timer.OnBestTimeUpdated += OnBestTimeUpdated;
    }

    void OnDisable()
    {
        Timer.OnBestTimeUpdated -= OnBestTimeUpdated;
    }

    void Start()
    {
        UpdateDisplay();
    }

    void OnBestTimeUpdated(int updatedLevel)
    {
        if (updatedLevel == levelNumber)
            UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        string key = "BestTime_Level" + levelNumber;
        if (PlayerPrefs.HasKey(key))
        {
            float best = PlayerPrefs.GetFloat(key);
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
