using UnityEngine;

public class AbilityPickup : MonoBehaviour
{
    public AbilityType ability = AbilityType.Dash;
    public float rotateSpeed = 60f;

    void Update()
    {
        transform.Rotate(0f, rotateSpeed * Time.deltaTime, 0f, Space.World);
    }

    void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<PlayerBallController>();
        if (player != null)
        {
            player.GiveAbility(ability);
            Destroy(gameObject);
        }
    }
}
