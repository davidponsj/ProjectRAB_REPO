using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class ShopManager : MonoBehaviour
{
    public int currentBallIndex;
    public GameObject[] ballModels;

    public BallBlueprint[] balls;
    public Button buyButton;
    public Button selectButton;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI buyPrice;

    void Start()
    {
        // Verificamos todas las bolas
        foreach (BallBlueprint ball in balls)
        {
            if (ball.price == 0)
            {
                // Skin gratuita
                ball.isUnlocked = true;
            }
            else
            {
                // Desbloqueada por compra normal o por recompensa bonus
                ball.isUnlocked = PlayerPrefs.GetInt(ball.name, 0) == 1 ||
                                  PlayerPrefs.GetInt("RewardBall_" + ball.index, 0) == 1;
            }

            Debug.Log("Leyendo skin: " + ball.name + " index: " + ball.index + " desbloqueada: " + ball.isUnlocked);
        }

        // Bola actualmente seleccionada
        currentBallIndex = PlayerPrefs.GetInt("SelectedBall", 0);

        // Desactivamos todos los modelos
        foreach (GameObject ball in ballModels)
            ball.SetActive(false);

        ballModels[currentBallIndex].SetActive(true);
    }


    // Update is called once per frame
    void Update()
    {
        UpdateUI();
    }

    public void Awake()
    {
        UpdateUI();
    }

    public void Next()
    {
        ballModels[currentBallIndex].SetActive(false);

        currentBallIndex++; 
        if (currentBallIndex == ballModels.Length)
            currentBallIndex = 0;

        ballModels[currentBallIndex].SetActive(true);

        BallBlueprint b = balls[currentBallIndex];
        if(!b.isUnlocked)
            return;


        PlayerPrefs.SetInt("SelectedBall", currentBallIndex);
        UpdateUI();
    }

    public void Previous()
    {
        ballModels[currentBallIndex].SetActive(false);

        currentBallIndex--;
        if (currentBallIndex == -1)
            currentBallIndex = ballModels.Length -1;

        ballModels[currentBallIndex].SetActive(true);

        BallBlueprint b = balls[currentBallIndex];
        if (!b.isUnlocked)
            return;

        PlayerPrefs.SetInt("SelectedBall", currentBallIndex);
        UpdateUI();
    }

    public void UnlockBall()
    {
        BallBlueprint b = balls[currentBallIndex];

        PlayerPrefs.SetInt(b.name, 1);
        PlayerPrefs.SetInt("SelectedBall", currentBallIndex);
        b.isUnlocked = true;
        PlayerPrefs.SetInt("NumberOfCoins", PlayerPrefs.GetInt("NumberOfCoins", 0) - b.price);
    }

    private void UpdateUI()
    {
        coinsText.text = ": " + PlayerPrefs.GetInt("NumberOfCoins", 0);
        BallBlueprint b = balls[currentBallIndex];
        if (b.isUnlocked)
        {
            buyButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(true);
            buyPrice.gameObject.SetActive(false);
        }
        else
        {
            
            buyPrice.gameObject.SetActive(true);
            buyButton.gameObject.SetActive(true);
            buyPrice.text = b.price + " Monedas";
            selectButton.gameObject.SetActive(false);
            if(b.price <= PlayerPrefs.GetInt("NumberOfCoins", 0))
            {
                buyButton.interactable = true;
            }
            else
            {
                buyButton.interactable = false;
            }

        }
    }
}
