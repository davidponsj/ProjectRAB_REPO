using System.Collections;
using UnityEngine;

public class PlayerSplitter : MonoBehaviour
{
    [Header("Prefabs & settings")]
    public GameObject halfPrefab;
    public float splitForce = 6f;
    public float upwardForce = 2f;
    public float lifeAfterSplit = 2f;

    [Header("Respawn")]
    public MonoBehaviour respawnHandler; // arrastra aquí el script que tiene public void Respawn()
    public string respawnMethodName = "Respawn";
    public float respawnDelay = 0.5f;

    [Header("Opcional")]
    public bool hideOriginal = true;

    private bool isSplitting = false;

    public void Split(Vector3 hazardPosition)
    {
        if (isSplitting) return;
        isSplitting = true;
        StartCoroutine(DoSplit(hazardPosition));
    }

    private IEnumerator DoSplit(Vector3 hazardPosition)
    {
        if (hideOriginal)
        {
            var rends = GetComponentsInChildren<Renderer>();
            foreach (var r in rends) r.enabled = false;

            var cols = GetComponentsInChildren<Collider>();
            foreach (var c in cols) c.enabled = false;

            var rbs = GetComponentsInChildren<Rigidbody>();
            foreach (var rb in rbs) rb.isKinematic = true;
        }

        Vector3 center = transform.position;
        Vector3 dir = (center - hazardPosition).normalized;
        if (dir == Vector3.zero) dir = Vector3.up + Vector3.right;

        Vector3 perp = Vector3.Cross(dir, Vector3.up).normalized;
        if (perp == Vector3.zero) perp = transform.right;

        Vector3 posA = center + perp * 0.1f;
        Vector3 posB = center - perp * 0.1f;

        if (halfPrefab != null)
        {
            GameObject a = Instantiate(halfPrefab, posA, transform.rotation);
            GameObject b = Instantiate(halfPrefab, posB, transform.rotation);

            Rigidbody ra = a.GetComponent<Rigidbody>();
            Rigidbody rb = b.GetComponent<Rigidbody>();

            if (ra != null) ra.AddForce((dir + perp).normalized * splitForce + Vector3.up * upwardForce, ForceMode.Impulse);
            if (rb != null) rb.AddForce((dir - perp).normalized * splitForce + Vector3.up * upwardForce, ForceMode.Impulse);

            Destroy(a, lifeAfterSplit);
            Destroy(b, lifeAfterSplit);
        }
        else
        {
            Debug.LogWarning("halfPrefab no asignado en PlayerSplitter.");
        }

        if (respawnHandler != null)
        {
            yield return new WaitForSeconds(respawnDelay);
            respawnHandler.Invoke(respawnMethodName, 0f);
        }
        else
        {
            // Si no tienes respawn, destruimos y recargamos escena como fallback
            yield return new WaitForSeconds(respawnDelay);
            UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
        }

        Destroy(gameObject, 0.01f);
    }
}