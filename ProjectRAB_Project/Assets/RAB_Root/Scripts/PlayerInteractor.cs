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
        // Si el jugador toca un objeto con tag "PickUp"
        if (other.gameObject.CompareTag("PickUp"))
        {
            coins++; // Suma una moneda

            // Guarda el estado de la moneda específica
            PickUpItem item = other.gameObject.GetComponent<PickUpItem>();
            if (item != null)
            {
                PlayerPrefs.SetInt(item.uniqueID, 1); // Marca la moneda como recogida
            }

            Destroy(other.gameObject); // Elimina la moneda de la escena
            PlayerPrefs.SetInt("NumberOfCoins", coins); // Actualiza el total de monedas
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
