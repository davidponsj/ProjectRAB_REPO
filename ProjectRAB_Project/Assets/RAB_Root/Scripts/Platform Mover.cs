using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [HideInInspector] public Vector3 endPoint;
    [HideInInspector] public MovingPlatformSpawner spawner;
    public float speed = 3f;

    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, endPoint, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, endPoint) < 0.05f)
        {
            // Informa al spawner y se destruye
            if (spawner != null)
                spawner.PlatformReachedEnd(gameObject);
        }
    }
}
