using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class BonusLevelManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public float levelTime = 60f;
    private float currentTime;

    [Header("UI Settings")]
    public TextMeshProUGUI timerText; // Texto del contador

    [Header("PickUp Settings")]
    public string bonusPickupTag = "BonusPickUp";
    private int totalPickUps;
    private int collectedPickUps;

    [Header("Scene Settings")]
    public string destinationSceneName = "LevelWithSpawnPoints"; // Escena a cargar donde está el checkpoint

    [Header("Reward Settings")]
    public int rewardBallIndex = 1;

    [Header("Checkpoint Settings")]
    public string checkpointName; // Checkpoint en la escena de destino

    private bool levelEnded = false;

    public static string nextCheckpoint; // Variable estática para checkpoint

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

        // Actualizar texto del contador
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(currentTime / 60);
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

            if (currentTime <= 5)
                timerText.color = Color.red;
        }

        int remaining = GameObject.FindGameObjectsWithTag(bonusPickupTag).Length;
        collectedPickUps = totalPickUps - remaining;

        // Si recoge todas las monedas, ir al checkpoint
        if (totalPickUps > 0 && collectedPickUps >= totalPickUps)
        {
            LevelEnd();
        }

        // Si se acaba el tiempo, ir al checkpoint
        if (currentTime <= 0)
        {
            LevelEnd();
        }
    }

    void LevelEnd()
    {
        levelEnded = true;

        // Guardar checkpoint
        nextCheckpoint = checkpointName;

        // Desbloquear recompensa
        string rewardKey = "RewardBall_" + rewardBallIndex;
        if (PlayerPrefs.GetInt(rewardKey, 0) == 0)
        {
            PlayerPrefs.SetInt(rewardKey, 1);
            PlayerPrefs.Save();
        }

        Debug.Log("Nivel terminado: moviendo jugador al checkpoint.");

        // Cargar escena de destino y mover jugador al checkpoint
        SceneManager.sceneLoaded += MovePlayerToCheckpoint;
        SceneManager.LoadScene(destinationSceneName);
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