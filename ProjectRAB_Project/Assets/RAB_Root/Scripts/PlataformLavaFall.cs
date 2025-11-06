using UnityEngine;

public class PlataformLavaFall : MonoBehaviour
{
    // Variables de tipo Float
    public float timeToFall = 1f; // Tiempo en segundos antes de que la plataforma comience a bajar
    public float fallSpeed = 0.5f; // Velocidad de descenso de la plataforma
    public float minZ = -30f; // Valor mínimo del eje Z (donde se detendrá la plataforma)

    private bool isPlayerOnPlatform = false; // Si el jugador esta encima de la plataforma
    private float timeOnPlatform = 0f; // Contador que no se ve de tiempo cuando el jugador está sobre la plataforma
    private Vector3 originalPosition; // Posicion inicial de la plataforma

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Guardar la posición original de la plataforma
        originalPosition = transform.position;
    }

    // Update se llama una vez por frame
    void Update()
    {
        //Si el jugador está sobre la plataforma y ha pasado 1 segundo
        if (isPlayerOnPlatform)
        {
            timeOnPlatform += Time.deltaTime;

            //Si ha pasado el tiempo necesario se cae
            if (timeOnPlatform >= timeToFall && isPlayerOnPlatform)
            {
                //Se mueve la plataforma lentamente hacia abajo en el eje Z
                if (transform.position.z > minZ)
                {
                    transform.position = new Vector3(
                        transform.position.x,
                        transform.position.y,
                        Mathf.Lerp(transform.position.z, minZ, fallSpeed * Time.deltaTime)
                    );
                }
            }
        }
        else
        {
            //Si el jugador se quita de la plataforma, se reinicia el contador
            timeOnPlatform = 0f;

            //Si la plataforma no esta en su punto de origen, se vuelve a su posicion original
            if (transform.position.z < originalPosition.z)
            {
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y,
                    Mathf.Lerp(transform.position.z, originalPosition.z, fallSpeed * Time.deltaTime)
                );
            }
        }
    }

    //Detecta cuando el jugador se pone encima de la plataforma
    private void OnCollisionEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnPlatform = true;
        }
    }

    //Detecta cuando el jugador sale de la plataforma
    private void OnCollisionExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerOnPlatform = false;
        }
    }
}
