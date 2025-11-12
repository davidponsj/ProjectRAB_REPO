using UnityEngine;

[System.Serializable]
public class BallBlueprint
{
    public string name;
    public int index;
    public int price;
    public bool isUnlocked;

    [Header("Bonus Reward")]
    public bool isReward = false; // true si se desbloquea solo en nivel bonus
}
