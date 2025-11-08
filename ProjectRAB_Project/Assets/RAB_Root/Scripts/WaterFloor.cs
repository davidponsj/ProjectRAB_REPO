using UnityEngine;

[RequireComponent(typeof(Collider))]
public class WaterFloor : MonoBehaviour
{
    [Header("Dirección y fuerza del empuje")]
    public Vector3 pushDirection = new Vector3(0f, 0f, 0f);             // Dirección del empuje
    public float pushStrength = 2f;                                     // Intensidad del empuje

    private void OnCollisionStay(Collision collision)
    {
        Rigidbody rb = collision.rigidbody;

        if (rb != null)
        {
                                                                        // Normalizamos la dirección por seguridad
            Vector3 push = pushDirection.normalized * pushStrength;

                                                // Añadimos una pequeña fuerza constante mientras esté en contacto
            rb.AddForce(push, ForceMode.Acceleration);
        }
    }
}