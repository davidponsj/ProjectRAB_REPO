using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BallSquashStretchNatural : MonoBehaviour
{
    [Header("Factores de Squash & Stretch")]
    public float squashAmount = 0.5f;    // cuánto se aplasta al caer
    public float stretchAmount = 0.5f;   // cuánto se estira al saltar
    public float smoothSpeed = 5f;       // suavizado de la transición

    private Rigidbody rb;
    private Vector3 originalScale;
    private bool isGrounded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        originalScale = transform.localScale;
    }

    void Update()
    {
        Vector3 targetScale = originalScale;

        if (isGrounded && rb.linearVelocity.y <= 0f)
        {
            // Aplastar al tocar el suelo
            targetScale = new Vector3(originalScale.x * (1f + squashAmount), originalScale.y * (1f - squashAmount), originalScale.z * (1f + squashAmount));
        }
        else if (!isGrounded && rb.linearVelocity.y > 0f)
        {
            // Estirar al saltar
            targetScale = new Vector3(originalScale.x * (1f - stretchAmount), originalScale.y * (1f + stretchAmount), originalScale.z * (1f - stretchAmount));
        }

        // Suavizar transición
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * smoothSpeed);
    }

    void OnCollisionEnter(Collision collision)
    {
        // Detectamos si tocamos suelo
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        // Salimos del suelo
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}
