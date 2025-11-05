using UnityEngine;

public class SandSpeed : MonoBehaviour
{
    public float reducedSpeed = 2f;  // Velocidad reducida cuando está en la arena
    public float normalSpeed = 5f;   // Velocidad nosrmal fuera de la arena
    private CharacterController characterController;  // Referencia al CharacterController del jugador

    // Para controlar si el jugador está en la arena
    private bool isInSand = false;

    void Start()
    {
        // Obtener el CharacterController del jugador
        characterController = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Comprobar si estamos en la arena y reducir la velocidad
        if (isInSand)
        {
            MovePlayer(reducedSpeed);
        }
        else
        {
            MovePlayer(normalSpeed);
        }
    }

    // Función para mover al jugador con la velocidad deseada
    void MovePlayer(float speed)
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontal, 0, vertical);
        direction.Normalize();

        // Aplicar el movimiento
        characterController.Move(direction * speed * Time.deltaTime);
    }

    // Detectar cuando el jugador entra en la arena
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sand"))
        {
            isInSand = true;
        }
    }

    // Detectar cuando el jugador sale de la arena
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Sand"))
        {
            isInSand = false;
        }
    }
}

