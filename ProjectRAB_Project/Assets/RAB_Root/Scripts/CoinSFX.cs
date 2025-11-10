using UnityEngine;

public class CoinSFX : MonoBehaviour
{
    public AudioClip coinClip; // Clip de recogida

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return; // solo Player

        // Reproducir SFX vía PersistentAudioManager
        if (PersistentAudioManager.Instance != null)
        {
            PersistentAudioManager.Instance.PlaySFX(coinClip);
        }
        else
        {
            // fallback si no hay manager
            AudioSource local = other.GetComponent<AudioSource>();
            if (local == null) local = other.gameObject.AddComponent<AudioSource>();
            local.PlayOneShot(coinClip);
        }

        // Aquí destruyes la moneda
        Destroy(gameObject);
    }
}