using UnityEngine;
public class LogBehavior : MonoBehaviour
{                   // ESTE SCRIPT SE LE AÑADE AL PREFAB DEL TRONCO
    public float minYToDestroy = 1f;       // Altura mínima antes de desaparecer

    void Update()
    {
        if (transform.position.y < minYToDestroy)
        {
            Destroy(gameObject);
        }
    }
}
