using UnityEngine;

public class BallRotator : MonoBehaviour
{
    public float rotationSpeed = 50;

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}
