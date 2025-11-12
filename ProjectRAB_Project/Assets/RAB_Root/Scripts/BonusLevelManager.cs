using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BonusLevelManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public float levelTime = 60f; // Duración del nivel
    private float currentTime;

    [Header("UI Settings")]
    public TextMeshProUGUI timerText; // Referencia al texto en pantalla

    [Header("PickUp Settings")]
    public string bonusPickupTag = "BonusPickUp";
    private int totalPickUps;
    private int collectedPickUps;

    [Header("Scene Settings")]
    public string failSceneName = "LevelWithSpawnPoints";
    public string successSceneName = "SuccessScene";

    [Header("Reward Settings")]
    public int rewardBallIndex = 1;

    [Header("Checkpoint Settings")]
    public string checkpointName; // Checkpoint de este bonus en la escena de destino

    private bool levelEnded = false;

    // Variable estática para pasar el checkpoint entre escenas
    public static string nextCheckpoint;

    void Start()
    {
        currentTime = levelTime;

        GameObject[] pickups = GameObject.FindGameObjectsWithTag(bonusPickupTag);
        totalPickUps = pickups.Length;
        collectedPickUps = 0;
    }

    void Update()
    {
        if (levelEnded) return;

        currentTime -= Time.deltaTime;

        // Actualizamos el contador en pantalla
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (currentTime <= 5) // Opcional: cambiar color si queda poco tiempo
                timerText.color = Color.red;
        }

        int remaining = GameObject.FindGameObjectsWithTag(bonusPickupTag).Length;
        collectedPickUps = totalPickUps - remaining;

        // Nivel completado
        if (totalPickUps > 0 && collectedPickUps >= totalPickUps)
        {
            LevelCompleted();
        }

        // Tiempo agotado
        if (currentTime <= 0)
        {
            LevelFailed();
        }
    }

    void LevelCompleted()
    {
        levelEnded = true;

        string rewardKey = "RewardBall_" + rewardBallIndex;
        if (PlayerPrefs.GetInt(rewardKey, 0) == 0)
        {
            PlayerPrefs.SetInt(rewardKey, 1);
            PlayerPrefs.Save();
        }

        SceneManager.LoadScene(successSceneName);
    }

    void LevelFailed()
    {
        levelEnded = true;

        // Guardamos el checkpoint
        nextCheckpoint = checkpointName;

        // Cargamos la escena de destino
        SceneManager.sceneLoaded += MovePlayerToCheckpoint;
        SceneManager.LoadScene(failSceneName);
    }

    void MovePlayerToCheckpoint(Scene scene, LoadSceneMode mode)
    {
        GameObject player = GameObject.FindWithTag("Player");
        GameObject checkpoint = GameObject.Find(nextCheckpoint);

        if (player != null && checkpoint != null)
        {
            player.transform.position = checkpoint.transform.position;
        }
        else
        {
            Debug.LogWarning("Jugador o checkpoint no encontrado");
        }

        SceneManager.sceneLoaded -= MovePlayerToCheckpoint;
    }
}