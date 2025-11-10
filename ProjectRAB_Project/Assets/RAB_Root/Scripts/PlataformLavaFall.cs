using UnityEngine;

public class PlataformLavaFall : MonoBehaviour
{
    [Header("Caída")]
    public float timeToFall = 1f; // Tiempo total antes de caer (acumulado)
    public float fallSpeed = 0.5f; // Velocidad de caída
    public float minY = -30f; // Límite inferior de caída

    [Header("Temblor")]
    public float shakeIntensityX = 0.05f;
    public float shakeIntensityZ = 0.05f;
    public float shakeSpeed = 30f;

    [Header("Reinicio")]
    public float resetDelay = 3f; // Tiempo antes de volver a subir

    private bool isPlayerOnPlatform = false;
    private float timeOnPlatform = 0f;
    private Vector3 originalPosition;
    private bool hasFallen = false;
    private bool isResetting = false;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        // Si el jugador está sobre la plataforma y aún no ha caído, acumula el tiempo
        if (isPlayerOnPlatform && !hasFallen)
        {
            timeOnPlatform += Time.deltaTime;

            // --- TEMBLOR ---
            float shakeOffsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensityX;
            float shakeOffsetZ = Mathf.Cos(Time.time * shakeSpeed) * shakeIntensityZ;
            Vector3 shakenPosition = originalPosition + new Vector3(shakeOffsetX, 0f, shakeOffsetZ);

            // --- CAÍDA ---
            if (timeOnPlatform >= timeToFall)
            {
                hasFallen = true;
            }
            else
            {
                // Solo tiembla sin caer todavía
                transform.position = new Vector3(shakenPosition.x, originalPosition.y, shakenPosition.z);
            }
        }

        // Si la plataforma está cayendo
        if (hasFallen && !isResetting)
        {
            if (transform.position.y > minY)
            {
                float newY = Mathf.Lerp(transform.position.y, minY, fallSpeed * Time.deltaTime);
                transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            }
            else
            {
                // Cuando llega abajo, inicia la corrutina de reinicio
                StartCoroutine(ResetPlatform());
            }
        }

        // Si el jugador se baja y la plataforma aún no ha caído, vuelve lentamente al centro
        if (!isPlayerOnPlatform && !hasFallen)
        {
            transform.position = new Vector3(
                Mathf.Lerp(transform.position.x, originalPosition.x, fallSpeed * Time.deltaTime),
                Mathf.Lerp(transform.position.y, originalPosition.y, fallSpeed * Time.deltaTime),
                Mathf.Lerp(transform.position.z, originalPosition.z, fallSpeed * Time.deltaTime)
            );
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = false;
        }
    }

    // Corrutina para hacer que la plataforma espere y vuelva a subir
    private System.Collections.IEnumerator ResetPlatform()
    {
        isResetting = true;
        yield return new WaitForSeconds(resetDelay);

        // Sube suavemente a la posición original
        while (Vector3.Distance(transform.position, originalPosition) > 0.05f)
        {
            transform.position = Vector3.Lerp(transform.position, originalPosition, fallSpeed * Time.deltaTime);
            yield return null;
        }

        // Restablecer estados
        transform.position = originalPosition;
        timeOnPlatform = 0f;
        hasFallen = false;
        isResetting = false;
    }
}
