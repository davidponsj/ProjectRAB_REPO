using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Tronco : MonoBehaviour
{
    public string playerTag = "Player";

    // Quita Reset() para no forzar isTrigger

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.CompareTag(playerTag)) return;

        var splitter = collision.collider.GetComponent<PlayerSplitter>();
        if (splitter != null)
            splitter.Split(transform.position);
        else
            Debug.LogWarning("El Player no tiene PlayerSplitter.");
    }
}
