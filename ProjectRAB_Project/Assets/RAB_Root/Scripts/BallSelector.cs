using UnityEngine;

public class BallSelector : MonoBehaviour
{
    public int currentBallIndex;
    public GameObject[] balls;

    void Start()
    {
        currentBallIndex = PlayerPrefs.GetInt("SelectedBall", 0);
        foreach (GameObject ball in balls)
            ball.SetActive(false);

        balls[currentBallIndex].SetActive(true);
    }
}
