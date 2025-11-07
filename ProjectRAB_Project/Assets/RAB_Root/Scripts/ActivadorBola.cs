using UnityEngine;

public class ActivadorBola : MonoBehaviour
{
    [Header("Referencias")]
    public Rigidbody boulderRb;      // asigná la bola aquí desde el Inspector

    [Header("Parámetros de velocidad")]
    public float gravityBoost = 2f;  // multiplica la gravedad para que caiga más rápido
    public float forwardForce = 30f; // fuerza extra hacia adelante (pendiente abajo)

    private bool isActive = false;

    void Start()
    {
        // Al inicio la bola queda congelada hasta activarse
        if (boulderRb != null)
            boulderRb.isKinematic = true;
    }

    void OnTriggerEnter(Collider other)
    {
        // Si el player entra en el trigger, activamos la bola
        if (other.CompareTag("Player"))
        {
            ActivateBoulder();
        }
    }

    void FixedUpdate()
    {
        if (isActive && boulderRb != null)
        {
            // Aumenta la gravedad artificialmente
            boulderRb.AddForce(Physics.gravity * gravityBoost, ForceMode.Acceleration);

            // Aplica un impulso extra hacia adelante
            boulderRb.AddForce(Vector3.forward * forwardForce, ForceMode.Force);
        }
    }

    void ActivateBoulder()
    {
        isActive = true;
        boulderRb.isKinematic = false;  // ahora la física lo mueve
    }
}
