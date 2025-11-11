using UnityEngine;
using TMPro;
using System;

public class Timer : MonoBehaviour
{
    public static event Action<int> OnBestTimeUpdated; // notifica cambios de best time

    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] int levelNumber = 1;

    float elapsedTime;
    bool isRunning = true;

    void Start()
    {
        elapsedTime = 0f;
        isRunning = true;
    }

    void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    // Llamar cuando el jugador termina el nivel
    public void StopTimer()
    {
        if (!isRunning) return;
        isRunning = false;
        bool savedNewBest = SaveBestTime(elapsedTime);

        if (savedNewBest)
        {
            Debug.Log($"Nuevo mejor tiempo guardado para nivel {levelNumber}: {FormatTime(elapsedTime)}");
            OnBestTimeUpdated?.Invoke(levelNumber); // notifica a la UI
        }
        else
        {
            Debug.Log($"Tiempo final para nivel {levelNumber}: {FormatTime(elapsedTime)} (no mejora)");
        }
    }

    // Devuelve true si se guardó nuevo mejor tiempo
    bool SaveBestTime(float newTime)
    {
        string key = "BestTime_Level" + levelNumber;

        if (PlayerPrefs.HasKey(key))
        {
            float bestTime = PlayerPrefs.GetFloat(key);

            if (newTime < bestTime)
            {
                PlayerPrefs.SetFloat(key, newTime);
                PlayerPrefs.Save();
                return true;
            }
            return false;
        }
        else
        {
            PlayerPrefs.SetFloat(key, newTime);
            PlayerPrefs.Save();
            return true;
        }
    }

    public static string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public string GetBestTime()
    {
        string key = "BestTime_Level" + levelNumber;
        if (PlayerPrefs.HasKey(key))
        {
            float best = PlayerPrefs.GetFloat(key);
            return FormatTime(best);
        }
        return "00:00";
    }
}
