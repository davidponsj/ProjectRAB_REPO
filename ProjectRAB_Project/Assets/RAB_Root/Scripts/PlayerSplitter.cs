using System.Collections;
using UnityEngine;

public class PlayerSplitter : MonoBehaviour
{
    [Header("Prefabs & settings")]
    public GameObject halfPrefab;
    public float splitForce = 6f;
    public float upwardForce = 2f;
    public float lifeAfterSplit = 2f;

    [Header("Respawn (opcional)")]
    public float respawnDelay = 0.5f; // Tiempo antes del respawn
    public Transform respawnPoint;     // Lugar donde reaparece el player

    private bool isSplitting = false;

    public void Split(Vector3 hazardPosition)
    {
        if (isSplitting) return;
        isSplitting = true;
        StartCoroutine(DoSplit(hazardPosition));
    }

    private IEnumerator DoSplit(Vector3 hazardPosition)
    {
        // Oculta renderers y desactiva colliders para la bola original
        var rends = GetComponentsInChildren<Renderer>();
        foreach (var r in rends) r.enabled = false;

        var cols = GetComponentsInChildren<Collider>();
        foreach (var c in cols) c.enabled = false;

        var rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;

        // Dirección del golpe
        Vector3 center = transform.position;
        Vector3 dir = (center - hazardPosition).normalized;
        if (dir == Vector3.zero) dir = Vector3.up + Vector3.right;
        Vector3 perp = Vector3.Cross(dir, Vector3.up).normalized;
        if (perp == Vector3.zero) perp = transform.right;

        // Crear las dos mitades
        Vector3 posA = center + perp * 0.1f;
        Vector3 posB = center - perp * 0.1f;

        if (halfPrefab != null)
        {
            GameObject a = Instantiate(halfPrefab, posA, transform.rotation);
            GameObject b = Instantiate(halfPrefab, posB, transform.rotation);

            Rigidbody ra = a.GetComponent<Rigidbody>();
            Rigidbody rb2 = b.GetComponent<Rigidbody>();

            if (ra != null) ra.AddForce((dir + perp).normalized * splitForce + Vector3.up * upwardForce, ForceMode.Impulse);
            if (rb2 != null) rb2.AddForce((dir - perp).normalized * splitForce + Vector3.up * upwardForce, ForceMode.Impulse);

            Destroy(a, lifeAfterSplit);
            Destroy(b, lifeAfterSplit);
        }
        else
        {
            Debug.LogWarning("PlayerSplitter: halfPrefab no asignado en " + gameObject.name);
        }

        // Esperar antes de respawnear
        yield return new WaitForSeconds(respawnDelay);

        // ✅ Respawn sin recargar la escena
        RespawnPlayer();
    }

    private void RespawnPlayer()
    {
        // Reset del player al punto de respawn
        transform.position = respawnPoint != null ? respawnPoint.position : Vector3.zero;
        transform.rotation = Quaternion.identity;

        // Reactivar renderers y colliders
        var rends = GetComponentsInChildren<Renderer>();
        foreach (var r in rends) r.enabled = true;

        var cols = GetComponentsInChildren<Collider>();
        foreach (var c in cols) c.enabled = true;

        var rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = false;

        isSplitting = false;
    }
}