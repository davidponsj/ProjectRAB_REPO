using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerSurfaceSFX : MonoBehaviour
{
    public AudioClip groundClip;
    public AudioClip waterClip;
    public AudioClip lavaClip;

    public float minImpactSpeed = 0.1f;
    public float maxImpactSpeed = 10f;
    public LayerMask groundLayer;

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if ((groundLayer.value & (1 << collision.gameObject.layer)) == 0) return;

        float impactSpeed = Mathf.Abs(rb.linearVelocity.y);
        if (impactSpeed < minImpactSpeed) return;

        AudioClip clipToPlay = groundClip;
        if (collision.collider.CompareTag("Water")) clipToPlay = waterClip;
        else if (collision.collider.CompareTag("Lava")) clipToPlay = lavaClip;

        float volume = Mathf.Clamp01((impactSpeed - minImpactSpeed) / (maxImpactSpeed - minImpactSpeed));

        if (PersistentAudioManager.Instance != null)
            PersistentAudioManager.Instance.PlaySFX(clipToPlay);
        else
        {
            AudioSource local = GetComponent<AudioSource>();
            if (local == null) local = gameObject.AddComponent<AudioSource>();
            local.PlayOneShot(clipToPlay, volume);
        }
    }
}