using UnityEngine;

public class PersistentAudioManager : MonoBehaviour
{
    public static PersistentAudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource; // Asigna el AudioSource de música
    public AudioSource sfxSource;   // Asigna el AudioSource de efectos

    [Header("Volúmenes iniciales")]
    [Range(0f, 1f)]
    public float CurrentMusicVolume = 1f;
    [Range(0f, 1f)]
    public float CurrentSFXVolume = 1f;

    void Awake()
    {
        // Singleton: si ya existe, destruye el duplicado
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Aplicar los volúmenes iniciales
        ApplyVolumes();
    }

    // Ajusta el volumen de música
    public void SetMusicVolume(float vol)
    {
        CurrentMusicVolume = Mathf.Clamp01(vol);
        ApplyVolumes();
    }

    // Ajusta el volumen de efectos
    public void SetSFXVolume(float vol)
    {
        CurrentSFXVolume = Mathf.Clamp01(vol);
        ApplyVolumes();
    }

    // Aplica los volúmenes a los AudioSources
    private void ApplyVolumes()
    {
        if (musicSource != null)
            musicSource.volume = CurrentMusicVolume;

        if (sfxSource != null)
            sfxSource.volume = CurrentSFXVolume;
    }

    // Reproduce un clip de efecto de sonido
    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip, CurrentSFXVolume);
    }

    // Reproduce un clip de música (opcional)
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (clip != null && musicSource != null)
        {
            musicSource.clip = clip;
            musicSource.loop = loop;
            musicSource.Play();
        }
    }
}