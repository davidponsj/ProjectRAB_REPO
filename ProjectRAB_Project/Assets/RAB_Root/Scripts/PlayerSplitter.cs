using System.Collections;
using UnityEngine;

public class PlayerSplitter : MonoBehaviour
{
    [Header("Prefabs & settings")]
    public GameObject halfPrefab;        // Prefab de la mitad de la bola
    public float splitForce = 6f;        // Fuerza al separarlas
    public float upwardForce = 2f;       // Fuerza hacia arriba
    public float lifeAfterSplit = 2f;    // Cuánto duran las mitades

    [Header("Respawn (opcional)")]
    public float respawnDelay = 0.5f;    // Tiempo antes del respawn

    private bool isSplitting = false;

    // Llamar desde el AxeHazard con la posición del hazard
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

        // Esperar un poco y reiniciar la escena como respawn simple
        yield return new WaitForSeconds(respawnDelay);
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}