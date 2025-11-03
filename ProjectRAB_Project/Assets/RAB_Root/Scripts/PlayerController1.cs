using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Editor References")]
    public Rigidbody playerRb; //Almacén del rigidbody del jugador para movimiento fisico

    [Header("Movement Parameters")]
    public float speed = 10; //Velocidad del personaje
    public Vector2 moveInput; //Almacén del valor de los botones de movimiento

    [Header("Jump Parameters")]
    public float jumpForce = 5; //Potencia de salto del personaje
    public bool isGrounded = true; //Define si el personaje puede saltar (estar en el suelo)

    [Header("Respawn System")]
    public float fallLimit = -10f; //Limite en -y que el personaje calcula para respawnear
    public Transform respawPoint; //Referencia a la posición de respawn





    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        //CinematicMovement();
        if (transform.position.y <= fallLimit)
        {
            Respawn();
        }
    }

    private void FixedUpdate()
    {
        //Update para movimientos por fisica
        PhyicalMovement();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; //Resetear la posibilidad de salto
        }
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            Respawn();
        }
    }


    void CinematicMovement()
    {
        //Recordar conjelar los ejes de rotación del rigibody
        //Time.deltaTime es una marcas de tiempo que normaliza el movimiento cinematico
        transform.Translate(Vector3.right * speed * moveInput.x * Time.deltaTime);
        transform.Translate(Vector3.forward * speed * moveInput.y * Time.deltaTime);
    }

    void PhyicalMovement()
    {
        //Descongelar los ejes de rotación del rigibody
        playerRb.AddForce(Vector3.right * speed * moveInput.x);
        playerRb.AddForce(Vector3.forward * speed * moveInput.y);
    }

    void Jump()
    {
        playerRb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    void Respawn()
    {
        //Sustituir la posición del player por la posición del punto respawn
        transform.position = respawPoint.position;
        //Resetear la energia de aceleración del rigibody
        playerRb.linearVelocity = new Vector3(0, 0, 0);
    }

    #region Input Methods

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded == true)
        {
            isGrounded = false;
            Jump();
        }
    }



    #endregion






}
