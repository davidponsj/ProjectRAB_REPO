using UnityEngine;

public class PlataformLavaFallMove : MonoBehaviour
{
    // Variables de tipo Float
    public float timeToFall = 1f; // Tiempo acumulado antes de que la plataforma comience a bajar
    public float fallSpeed = 0.5f; // Velocidad de descenso de la plataforma
    public float minY = -500f; // Valor mínimo del eje Y (donde se detendrá la plataforma)
    public float shakeIntensity = 0.05f; // Intensidad del temblor en el eje Z
    public float shakeSpeed = 25f; // Velocidad del temblor (frecuencia de oscilación)

    private bool isPlayerOnPlatform = false; // Si el jugador está encima de la plataforma
    private float accumulatedTime = 0f; // Tiempo total acumulado mientras el jugador está en la plataforma
    private Vector3 originalPosition; // Posición inicial de la plataforma
    private float shakeOffset = 0f; // Desfase del temblor para evitar reinicios bruscos

    void Start()
    {
        originalPosition = transform.position;
    }

    void Update()
    {
        Vector3 newPosition = transform.position;

        if (isPlayerOnPlatform)
        {
            // Acumular tiempo mientras el jugador está encima
            accumulatedTime += Time.deltaTime;

            // Aplicar temblor en Z
            shakeOffset += Time.deltaTime * shakeSpeed;
            float shakeZ = Mathf.Sin(shakeOffset) * shakeIntensity;
            newPosition.z = originalPosition.z + shakeZ;

            // Si el tiempo acumulado alcanza o supera el tiempo de caída
            if (accumulatedTime >= timeToFall)
            {
                // Reiniciar contador
                accumulatedTime = 0f;

                // Mover la plataforma hacia abajo
                if (transform.position.y > minY)
                {
                    newPosition.y = Mathf.Lerp(transform.position.y, minY, fallSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            // Al salir del jugador, quitar temblor suavemente
            shakeOffset = 0f;
            newPosition.z = Mathf.Lerp(transform.position.z, originalPosition.z, Time.deltaTime * 10f);

            // La plataforma vuelve a su posición original
            if (transform.position.y < originalPosition.y)
            {
                newPosition.y = Mathf.Lerp(transform.position.y, originalPosition.y, fallSpeed * Time.deltaTime);
            }
        }

        transform.position = newPosition;
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

