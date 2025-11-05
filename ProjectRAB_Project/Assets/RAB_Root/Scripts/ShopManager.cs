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

    void Start()
    {
        foreach(BallBlueprint ball in balls)
        {
            if (ball.price == 0)
                ball.isUnlocked = true;
            else
                ball.isUnlocked = PlayerPrefs.GetInt(ball.name, 0)==0 ? false: true;

        }

        currentBallIndex = PlayerPrefs.GetInt("SelectedBall", 0);
        foreach(GameObject ball in ballModels)
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

    public void UnlockCar()
    {
        BallBlueprint b = balls[currentBallIndex];

        PlayerPrefs.SetInt(b.name, 1);
        PlayerPrefs.SetInt("SelectedBall", currentBallIndex);
        b.isUnlocked = true;
        PlayerPrefs.SetInt("NumberOfCoins", PlayerPrefs.GetInt("NumberOfCoins", 0) - b.price);
    }

    private void UpdateUI()
    {
        coinsText.text = "Monedas: " + PlayerPrefs.GetInt("NumberOfCoins", 0);
        BallBlueprint b = balls[currentBallIndex];
        if (b.isUnlocked)
        {
            buyButton.gameObject.SetActive(false);
            selectButton.gameObject.SetActive(true);
           
        }
        else
        {
            buyButton.gameObject.SetActive(true);
            selectButton.gameObject.SetActive(false);
            buyButton.GetComponentInChildren<TextMeshProUGUI>().text = "Buy-" + b.price;
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
