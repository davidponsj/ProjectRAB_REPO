using UnityEngine;

public class PlayerJumpEffect : MonoBehaviour
{
    [Header("Salto")]
    public float jumpForce = 5f;

    [Header("Efecto de polvo")]
    public GameObject dustEffectPrefab; // prefab con Particle System configurado para disolver

    private Rigidbody rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Saltar si está en el suelo
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            SpawnDustEffect(); // efecto al despegar
            isGrounded = false;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        // Detectar suelo
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            SpawnDustEffect(); // efecto al aterrizar
        }
    }

    void SpawnDustEffect()
    {
        if (dustEffectPrefab != null)
        {
            // Posición debajo de la bola
            Vector3 spawnPos = transform.position + Vector3.down * 0.5f;

            // Instancia el prefab
            GameObject effect = Instantiate(dustEffectPrefab, spawnPos, Quaternion.identity);

            // Destruye el prefab después de que todas las partículas se hayan disuelto
            Destroy(effect, 2f);
        }
    }
}
