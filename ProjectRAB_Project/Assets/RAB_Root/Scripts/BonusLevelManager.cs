using UnityEngine;
using UnityEngine.SceneManagement;

public class BonusLevelManager : MonoBehaviour
{
    [Header("Timer Settings")]
    public float levelTime = 60f;
    private float currentTime;

    [Header("PickUp Settings")]
    public string bonusPickupTag = "BonusPickUp"; // Tag de los pickups del nivel bonus
    private int totalPickUps;
    private int collectedPickUps;

    [Header("Scene Settings")]
    public int failSceneIndex = 3;
    public int successSceneIndex = 4;

    [Header("Reward Settings")]
    public int rewardBallIndex = 1; // Debe coincidir con BallBlueprint.index en ShopManager

    private bool levelEnded = false;

    void Start()
    {
        currentTime = levelTime;

        // Contamos los pickups del nivel bonus al inicio
        GameObject[] pickups = GameObject.FindGameObjectsWithTag(bonusPickupTag);
        totalPickUps = pickups.Length;
        collectedPickUps = 0;

        if (totalPickUps == 0)
            Debug.LogWarning(" No se encontraron pickups con el tag '" + bonusPickupTag + "' en la escena.");

        Debug.Log("Inicio nivel bonus: " + totalPickUps + " pickups totales.");
    }

    void Update()
    {
        if (levelEnded) return;

        currentTime -= Time.deltaTime;

        // Contamos cuántos pickups quedan activos en escena
        int remaining = GameObject.FindGameObjectsWithTag(bonusPickupTag).Length;
        collectedPickUps = totalPickUps - remaining;

        // Debug en consola
        Debug.Log("Tiempo restante: " + Mathf.CeilToInt(currentTime) +
                  "s | Recogidos: " + collectedPickUps + " / " + totalPickUps);

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
            Debug.Log("Skin desbloqueada: " + rewardKey);
        }

        Debug.Log("Nivel bonus completado.");
        SceneManager.LoadScene(successSceneIndex);
    }

    void LevelFailed()
    {
        levelEnded = true;
        Debug.Log("No conseguiste todos los pickups a tiempo.");
        SceneManager.LoadScene(failSceneIndex);
    }
}
