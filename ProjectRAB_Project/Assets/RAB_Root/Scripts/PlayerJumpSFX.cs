using UnityEngine;

public class PlayerJumpSFX : MonoBehaviour
{
    public AudioClip jumpClip;            // arrastra aquí jump.wav
    public float jumpCooldown = 0.12f;    // evita repetir sonido muy rápido

    private float lastJumpTime = -10f;

    void Update()
    {
        // Detecta la pulsación de salto (usa Input por defecto)
        if (Input.GetButtonDown("Jump"))
        {
            TryPlayJump();
        }
    }

    void TryPlayJump()
    {
        if (Time.time - lastJumpTime < jumpCooldown) return;
        lastJumpTime = Time.time;

        if (jumpClip == null) return;

        if (PersistentAudioManager.Instance != null)
        {
            PersistentAudioManager.Instance.PlaySFX(jumpClip);
        }
        else
        {
            // Fallback controlado por PlayerPrefs (clave debe coincidir con la que usas)
            float sfxVol = 1f;
            if (PlayerPrefs.HasKey("SFXVolSimple"))
                sfxVol = PlayerPrefs.GetFloat("SFXVolSimple");
            // Reproducir localmente con volumen aplicado
            AudioSource local = GetComponent<AudioSource>();
            if (local == null) local = gameObject.AddComponent<AudioSource>();
            local.volume = Mathf.Clamp01(sfxVol);
            local.PlayOneShot(jumpClip);
            Debug.LogWarning("PersistentAudioManager no encontrado. Reproduciendo localmente con PlayerPrefs SFXVolSimple=" + sfxVol);
        }
    }
}

