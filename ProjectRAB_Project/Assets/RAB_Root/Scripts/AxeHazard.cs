using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AxeHazard : MonoBehaviour
{
    public string playerTag = "Player";

    void Reset()
    {
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;

        var splitter = other.GetComponent<PlayerSplitter>();
        if (splitter != null)
            splitter.Split(transform.position);
        else
            Debug.LogWarning("El Player no tiene PlayerSplitter.");
    }
}
