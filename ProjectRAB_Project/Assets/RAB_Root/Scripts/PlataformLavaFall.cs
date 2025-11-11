using UnityEngine;

public class PlataformLavaFall : MonoBehaviour
{
    // Variables públicas configurables desde el inspector
    public float timeToFall = 1f; // Tiempo antes de caer
    public float fallSpeed = 0.5f; // Velocidad de caída
    public float minY = -500f; // Altura mínima
    public float shakeAmplitude = 0.1f; // Qué tanto se mueve de lado a lado
    public float shakeFrequency = 20f; // Qué tan rápido tiembla
    public float resetDuration = 3f; // Tiempo que tarda en volver a su posición original

    // Variables privadas
    private bool isPlayerOnPlatform = false;
    private float accumulatedTimeOnPlatform = 0f;
    private Vector3 originalPosition;
    private bool hasFallen = false;
    private bool isResetting = false; // Indica si está regresando a su posición
    private float resetTimer = 0f;
    private Vector3 resetStartPos;

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        // Si está en proceso de volver, manejar ese movimiento
        if (isResetting)
        {
            resetTimer += Time.deltaTime;
            float t = Mathf.Clamp01(resetTimer / resetDuration); // Normalizar tiempo [0,1]

            // Movimiento suave de regreso
            transform.position = Vector3.Lerp(resetStartPos, originalPosition, t);

            // Cuando termina el tiempo de reinicio
            if (t >= 1f)
            {
                transform.position = originalPosition;
                isResetting = false;
                hasFallen = false;
                accumulatedTimeOnPlatform = 0f;
                isPlayerOnPlatform = false;
            }

            return; // No ejecutar el resto del Update mientras vuelve
        }

        if (hasFallen)
            return;

        if (isPlayerOnPlatform)
        {
            accumulatedTimeOnPlatform += Time.deltaTime;

            // Si todavía no alcanzó el tiempo de caída, tiembla
            if (accumulatedTimeOnPlatform < timeToFall)
            {
                float shakeOffset = Mathf.Sin(Time.time * shakeFrequency) * shakeAmplitude;
                transform.position = new Vector3(
                    originalPosition.x + shakeOffset,
                    originalPosition.y,
                    originalPosition.z
                );
            }
            else
            {
                // Empieza a caer
                if (transform.position.y > minY)
                {
                    transform.position = new Vector3(
                        transform.position.x,
                        Mathf.Lerp(transform.position.y, minY, fallSpeed * Time.deltaTime),
                        transform.position.z
                    );
                }
                else
                {
                    hasFallen = true;
                }
            }

            // Si el jugador ha estado encima por al menos 1 segundo, comenzar reinicio
            if (accumulatedTimeOnPlatform >= 1f && !isResetting)
            {
                isResetting = true;
                resetTimer = 0f;
                resetStartPos = transform.position;
            }
        }
        else if (!hasFallen)
        {
            // Si el jugador se fue y no cayó, la plataforma vuelve suavemente
            transform.position = Vector3.Lerp(
                transform.position,
                originalPosition,
                fallSpeed * Time.deltaTime
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
}

