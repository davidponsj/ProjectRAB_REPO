using UnityEngine;

public class PlataformSandSlower : MonoBehaviour
{
    // Velocidad normal del jugador
    public float normalSpeed = 5f;

    // Velocidad reducida cuando está en la arena
    public float slowSpeed = 2f;

    // Componente Rigidbody del jugador
    private Rigidbody rb;

    // Bandera para verificar si el jugador está en la arena
    private bool isInSand = false;

    void Start()
    {
        // Obtener el Rigidbody del jugador
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Si está en la arena, reducir la velocidad
        if (isInSand)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, slowSpeed);
        }
        else
        {
            // Si no está en la arena, usar la velocidad normal
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, normalSpeed);
        }
    }

    // Cuando el jugador entra en contacto con la arena
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sand"))
        {
            isInSand = true; // El jugador está en la arena
        }
    }

    // Cuando el jugador deja de estar en contacto con la arena
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sand"))
        {
            isInSand = false; // El jugador salió de la arena
        }
    }
}
