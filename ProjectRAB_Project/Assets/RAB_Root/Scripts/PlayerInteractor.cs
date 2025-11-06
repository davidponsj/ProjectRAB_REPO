using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInteractor : MonoBehaviour
{
    [Header("Points System")]
    public static int coins; //Puntuación actual del player (en juego)
   // public int winPoints = 1; //Puntuación a alcanzar para completar el nivel
    public TMP_Text pointsText; //Ref al texto de puntos para que cambie dinámicamente

    [Header("Scene Management")]
    public int sceneToLoad = 2;

    [Header("Sound References")]
    public PlayerController playerCont; //Ref als cript que contiene las llamadas a sonidos

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        pointsText.text = "Monedas: " + coins.ToString();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PickUp"))
        {
            PlayerInteractor.coins++;
            //Destroy(other.gameObject);
            other.gameObject.SetActive(false);
            PlayerPrefs.SetInt("NumberOfCoins", PlayerInteractor.coins);
        }
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    private void Awake()
    {
        coins = PlayerPrefs.GetInt("NumberOfCoins", 0);
    }

}
