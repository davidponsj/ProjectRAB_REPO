using UnityEngine;

public class PlataformSandSlower : MonoBehaviour
{
    [Header("Velocidades")]
    public float velocidadNormal = 5f;
    public float velocidadReducida = 2f;

    private float velocidadActual;
    private CharacterController controller;

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        velocidadActual = velocidadNormal;
    }

    private void Update()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 movimiento = new Vector3(horizontal, 0, vertical);
        controller.Move(movimiento * velocidadActual * Time.deltaTime);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Si el jugador toca una plataforma con el tag "SlowPlatform"
        if (hit.collider.CompareTag("SlowPlatform"))
        {
            velocidadActual = velocidadReducida;
        }
        else
        {
            velocidadActual = velocidadNormal;
        }
    }
}
