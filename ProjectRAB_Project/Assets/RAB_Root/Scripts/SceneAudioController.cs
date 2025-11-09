using UnityEngine;
using UnityEngine.UI;

public class SceneAudioController : MonoBehaviour
{
    [Header("Sliders de audio en la escena")]
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        // Asegurarse de que el PersistentAudioManager existe
        if (PersistentAudioManager.Instance == null)
        {
            Debug.LogError("PersistentAudioManager no encontrado en la escena.");
            return;
        }

        // Conectar sliders a los volúmenes actuales
        if (musicSlider != null)
        {
            musicSlider.value = PersistentAudioManager.Instance.CurrentMusicVolume;
            musicSlider.onValueChanged.AddListener(PersistentAudioManager.Instance.SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = PersistentAudioManager.Instance.CurrentSFXVolume;
            sfxSlider.onValueChanged.AddListener(PersistentAudioManager.Instance.SetSFXVolume);
        }
    }

    void OnDestroy()
    {
        // Limpiar listeners para evitar errores al cambiar de escena
        if (PersistentAudioManager.Instance != null)
        {
            if (musicSlider != null)
                musicSlider.onValueChanged.RemoveListener(PersistentAudioManager.Instance.SetMusicVolume);

            if (sfxSlider != null)
                sfxSlider.onValueChanged.RemoveListener(PersistentAudioManager.Instance.SetSFXVolume);
        }
    }
}
