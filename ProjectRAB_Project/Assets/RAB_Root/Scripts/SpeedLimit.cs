using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SpeedLimit : MonoBehaviour
{
    [Header("Velocidad m�xima permitida")]
    public float maxSpeed = 10f;

    [Header("Limitar solo en el plano horizontal (X,Z)")]
    public bool limitHorizontalOnly = false;

    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (limitHorizontalOnly)
        {
            // Solo limitamos velocidad en el plano horizontal
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

            if (horizontalVelocity.magnitude > maxSpeed)
            {
                // Reescalamos solo la parte horizontal
                horizontalVelocity = horizontalVelocity.normalized * maxSpeed;
                rb.linearVelocity = new Vector3(horizontalVelocity.x, rb.linearVelocity.y, horizontalVelocity.z);
            }
        }
        else
        {
            // Limitamos velocidad total (todas las direcciones)
            if (rb.linearVelocity.magnitude > maxSpeed)
            {
                rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
            }
        }
    }
}

