using UnityEngine;
using TMPro;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] int levelNumber = 1; 

    float elapsedTime;
    bool isRunning = true;

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
        isRunning = false;
        SaveBestTime(elapsedTime);
    }

    void SaveBestTime(float newTime)
    {
        string key = "BestTime_Level" + levelNumber;

        if (PlayerPrefs.HasKey(key))
        {
            float bestTime = PlayerPrefs.GetFloat(key);

            // Guardamos solo si el nuevo tiempo es mejor (menor)
            if (newTime < bestTime)
            {
                PlayerPrefs.SetFloat(key, newTime);
                PlayerPrefs.Save();
                Debug.Log("Nuevo mejor tiempo para Nivel " + levelNumber + ": " + FormatTime(newTime));
            }
        }
        else
        {
            // Primera vez que se guarda para este nivel
            PlayerPrefs.SetFloat(key, newTime);
            PlayerPrefs.Save();
            Debug.Log("Primer tiempo guardado para Nivel " + levelNumber + ": " + FormatTime(newTime));
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
        float best = PlayerPrefs.GetFloat(key, 0f);
        return best > 0 ? FormatTime(best) : "00:00";
    }
}
