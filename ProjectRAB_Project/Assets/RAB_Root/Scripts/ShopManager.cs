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
        foreach (BallBlueprint ball in balls)
        {
            if (ball.price == 0)
            {
                ball.isUnlocked = true; // gratuita
            }
            else if (ball.isReward)
            {
                // Skin de recompensa: desbloqueada solo si se completó el nivel bonus
                ball.isUnlocked = PlayerPrefs.GetInt("RewardBall_" + ball.index, 0) == 1;
            }
            else
            {
                // Skin normal: desbloqueada si se compró
                ball.isUnlocked = PlayerPrefs.GetInt(ball.name, 0) == 1;
            }

            Debug.Log("Leyendo skin: " + ball.name + " index: " + ball.index + " desbloqueada: " + ball.isUnlocked + " isReward: " + ball.isReward);
        }

        currentBallIndex = PlayerPrefs.GetInt("SelectedBall", 0);

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
        BallBlueprint ball = balls[currentBallIndex];
        coinsText.text = ": " + PlayerPrefs.GetInt("NumberOfCoins", 0);

        if (ball.isUnlocked)
        {
            buyButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(true);
            buyPrice.gameObject.SetActive(false);
        }
        else
        {
            buyPrice.gameObject.SetActive(true);

            if (ball.isReward)
            {
                // Skin de recompensa bloqueada, no se puede comprar
                buyButton.gameObject.SetActive(false);
                buyPrice.text = "Desbloquea en nivel bonus";
            }
            else
            {
                // Skin normal comprable
                buyButton.gameObject.SetActive(true);
                buyPrice.text = ball.price + " Monedas";
                buyButton.interactable = ball.price <= PlayerPrefs.GetInt("NumberOfCoins", 0);
            }

            selectButton.gameObject.SetActive(false);
        }
    }

}
