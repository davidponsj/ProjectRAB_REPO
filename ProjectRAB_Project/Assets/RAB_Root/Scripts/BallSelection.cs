using UnityEngine;
using UnityEngine.UI;

public class BallSelection : MonoBehaviour
{
    [SerializeField] private Button previousButton;
    [SerializeField] private Button nextButton;
    private int currentBall;

    private void Awake()
    {
        SelectBall(0);
    }

    public void SelectBall(int _index)
    {
        previousButton.interactable = (_index != 0);
        nextButton.interactable = (_index != transform.childCount -1);
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(i == _index);
        }
    }

    public void ChangeBall(int _change)
    {
        currentBall += _change;
        SelectBall(currentBall);
    }
}
