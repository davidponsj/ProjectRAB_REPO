using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SceneAudioController : MonoBehaviour
{
    public bool treatAsSFX = true; // true = usar SFX volume, false = usar MUSIC volume (por ejemplo loops musicales)
    private AudioSource src;

    void Awake()
    {
        src = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        // Aplicar volumen actual al activarse
        if (PersistentAudioManager.Instance != null)
        {
            float v = treatAsSFX ? PersistentAudioManager.Instance.CurrentSFXVolume
                                 : PersistentAudioManager.Instance.CurrentMusicVolume;
            src.volume = v;

            // Suscribir al evento para cambios futuros
            if (treatAsSFX)
                PersistentAudioManager.Instance.OnSFXVolumeChanged += OnSFXVolumeChanged;
            else
                PersistentAudioManager.Instance.OnMusicVolumeChanged += OnMusicVolumeChanged;
        }
    }

    void OnDisable()
    {
        // Desuscribir para evitar memory leaks
        if (PersistentAudioManager.Instance != null)
        {
            if (treatAsSFX)
                PersistentAudioManager.Instance.OnSFXVolumeChanged -= OnSFXVolumeChanged;
            else
                PersistentAudioManager.Instance.OnMusicVolumeChanged -= OnMusicVolumeChanged;
        }
    }

    void OnSFXVolumeChanged(float newVol)
    {
        src.volume = newVol;
    }

    void OnMusicVolumeChanged(float newVol)
    {
        src.volume = newVol;
    }
}
