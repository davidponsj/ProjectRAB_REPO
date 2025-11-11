using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class Countdown : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] float remainingTime;

    [SerializeField] string sceneName;      // Escena a cargar
    [SerializeField] string checkpointName; // Nombre del GameObject checkpoint

    bool hasEnded = false;

    void Update()
    {
        if (remainingTime > 0)
        {
            remainingTime -= Time.deltaTime;

            if (remainingTime < 5)
                timerText.color = Color.red;
        }
        else if (!hasEnded)
        {
            remainingTime = 0;
            hasEnded = true;

            SceneManager.sceneLoaded += OnSceneLoaded; // Suscribirse al evento
            SceneManager.LoadScene(sceneName);         // Cargar escena
        }

        int minutes = Mathf.FloorToInt(remainingTime / 60);
        int seconds = Mathf.FloorToInt(remainingTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Encontrar jugador y checkpoint
        GameObject player = GameObject.FindWithTag("Player");
        GameObject checkpoint = GameObject.Find(checkpointName);

        if (player != null && checkpoint != null)
        {
            player.transform.position = checkpoint.transform.position;
        }

        SceneManager.sceneLoaded -= OnSceneLoaded; // Desuscribirse
    }
}

