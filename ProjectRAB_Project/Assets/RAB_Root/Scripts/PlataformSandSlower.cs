using System.Collections;
using UnityEngine;

public class PlataformSandSlower : MonoBehaviour
{
    // Velocidad normal del jugador
    public float normalSpeed ;

    // Velocidad reducida cuando está en la arena
    public float slowSpeed = 2f;

    public float moveForce ;

    public float slowmoveForce = 2;


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
    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            normalSpeed = other.gameObject.GetComponent<PlayerBallController>().MoveSpeed;
            moveForce = other.gameObject.GetComponent<PlayerBallController>().MoveForce;
            other.gameObject.GetComponent<PlayerBallController>().MoveSpeed = slowSpeed;
            other.gameObject.GetComponent<PlayerBallController>().MoveForce = slowmoveForce;
        }
    }

    // Cuando el jugador deja de estar en contacto con la arena
    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isInSand = false; // El jugador salió de la arena
            other.gameObject.GetComponent<PlayerBallController>().MoveSpeed = normalSpeed;
            other.gameObject.GetComponent<PlayerBallController>().MoveForce = moveForce;
        }
    }
}
