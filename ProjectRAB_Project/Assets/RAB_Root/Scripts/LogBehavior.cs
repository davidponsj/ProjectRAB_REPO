using UnityEngine;
public class LogBehavior : MonoBehaviour
{                   // ESTE SCRIPT SE LE AÑADE AL PREFAB DEL TRONCO
    public float minYToDestroy = 1f;       // Altura mínima antes de desaparecer
    public string playerTag = "Player";

    void Update()
    {
        if (transform.position.y < minYToDestroy)
        {
            Destroy(gameObject);
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(playerTag))
        {
                    // Destruir el tronco
            Destroy(gameObject);
        }
    }
}
