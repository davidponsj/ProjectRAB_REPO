using UnityEngine;

public class PlataformLavaFall : MonoBehaviour
{
    // Variables configurables desde el inspector
    [Header("Caída")]
    public float timeToFall = 1f;        // Tiempo antes de que comience a caer
    public float fallSpeed = 0.5f;       // Velocidad de caída
    public float minY = -30f;            // Altura mínima (límite inferior)

    [Header("Temblor")]
    public float shakeIntensityX = 0.05f; // Intensidad del temblor en X
    public float shakeIntensityZ = 0.05f; // Intensidad del temblor en Z
    public float shakeSpeed = 30f;        // Velocidad del temblor

    private bool isPlayerOnPlatform = false;
    private float timeOnPlatform = 0f;
    private Vector3 originalPosition;

    void Start()
    {
        // Guardar la posición original de la plataforma
        originalPosition = transform.position;
    }

    void Update()
    {
        // Si el jugador está sobre la plataforma
        if (isPlayerOnPlatform)
        {
            timeOnPlatform += Time.deltaTime;

            // --- TEMBLOR ---
            float shakeOffsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensityX;
            float shakeOffsetZ = Mathf.Cos(Time.time * shakeSpeed) * shakeIntensityZ;
            Vector3 shakenPosition = originalPosition + new Vector3(shakeOffsetX, 0f, shakeOffsetZ);

            // --- CAÍDA ---
            if (timeOnPlatform >= timeToFall)
            {
                if (transform.position.y > minY)
                {
                    float newY = Mathf.Lerp(transform.position.y, minY, fallSpeed * Time.deltaTime);
                    transform.position = new Vector3(shakenPosition.x, newY, shakenPosition.z);
                }
            }
            else
            {
                // Solo tiembla sin caer todavía
                transform.position = new Vector3(shakenPosition.x, originalPosition.y, shakenPosition.z);
            }
        }
        else
        {
            // Reiniciar el contador si el jugador se quita
            timeOnPlatform = 0f;

            // Volver a la posición original suavemente
            transform.position = new Vector3(
                Mathf.Lerp(transform.position.x, originalPosition.x, fallSpeed * Time.deltaTime),
                Mathf.Lerp(transform.position.y, originalPosition.y, fallSpeed * Time.deltaTime),
                Mathf.Lerp(transform.position.z, originalPosition.z, fallSpeed * Time.deltaTime)
            );
        }
    }

    // Detecta cuando el jugador se sube
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = true;
        }
    }

    // Detecta cuando el jugador se baja
    private void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPlayerOnPlatform = false;
        }
    }
}
