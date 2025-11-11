using UnityEngine;
using TMPro;

public class LevelTimer : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private TextMeshProUGUI currentTimeText; // Texto del tiempo en curso
    [SerializeField] private TextMeshProUGUI bestTimeText;    // Texto del mejor tiempo

    [Header("Nivel")]
    [SerializeField] private int levelNumber = 1; // Número del nivel (para PlayerPrefs)

    private float elapsedTime = 0f;
    private bool isRunning = true;

    void Start()
    {
        isRunning = true;
        elapsedTime = 0f;

        UpdateBestTimeText(); // Mostrar mejor tiempo al inicio
    }

    void Update()
    {
        if (!isRunning) return;

        elapsedTime += Time.deltaTime;

        // Actualiza el tiempo actual en pantalla
        currentTimeText.text = FormatTime(elapsedTime);
    }

    // Llamar cuando el jugador termine el nivel
    public void StopTimer()
    {
        if (!isRunning) return;

        isRunning = false;
        SaveBestTime();
    }

    private void SaveBestTime()
    {
        string key = "BestTime_Level" + levelNumber;
        bool isNewBest = false;

        if (PlayerPrefs.HasKey(key))
        {
            float best = PlayerPrefs.GetFloat(key);
            if (elapsedTime < best)
            {
                PlayerPrefs.SetFloat(key, elapsedTime);
                PlayerPrefs.Save();
                isNewBest = true;
            }
        }
        else
        {
            PlayerPrefs.SetFloat(key, elapsedTime);
            PlayerPrefs.Save();
            isNewBest = true;
        }

        if (isNewBest)
        {
            Debug.Log($"¡Nuevo mejor tiempo para Nivel {levelNumber}: {FormatTime(elapsedTime)}!");
            UpdateBestTimeText();
        }
    }

    private void UpdateBestTimeText()
    {
        string key = "BestTime_Level" + levelNumber;
        if (PlayerPrefs.HasKey(key))
        {
            float best = PlayerPrefs.GetFloat(key);
            bestTimeText.text = "Mejor tiempo: " + FormatTime(best);
        }
        else
        {
            bestTimeText.text = "Aún no hay mejor tiempo";
        }
    }

    private string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
