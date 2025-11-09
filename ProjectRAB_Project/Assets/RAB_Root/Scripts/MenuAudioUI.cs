using UnityEngine;
using UnityEngine.UI;

public class MenuAudioUI : MonoBehaviour
{
    public Slider musicSlider; // arrastra aquí el slider de música
    public Slider sfxSlider;   // arrastra aquí el slider de efectos

    void Start()
    {
        // Buscar al PersistentAudioManager que viene del tutorial
        var manager = PersistentAudioManager.Instance;
        if (manager == null)
        {
            Debug.LogError("PersistentAudioManager no encontrado");
            return;
        }

        // Conectar sliders a manager
        if (musicSlider != null)
        {
            musicSlider.value = manager.CurrentMusicVolume;
            musicSlider.onValueChanged.AddListener(manager.SetMusicVolume);
        }

        if (sfxSlider != null)
        {
            sfxSlider.value = manager.CurrentSFXVolume;
            sfxSlider.onValueChanged.AddListener(manager.SetSFXVolume);
        }

        Debug.Log("Sliders conectados al PersistentAudioManager");
    }

    void OnDestroy()
    {
        // Limpiar listeners
        if (PersistentAudioManager.Instance != null)
        {
            if (musicSlider != null)
                musicSlider.onValueChanged.RemoveListener(PersistentAudioManager.Instance.SetMusicVolume);

            if (sfxSlider != null)
                sfxSlider.onValueChanged.RemoveListener(PersistentAudioManager.Instance.SetSFXVolume);
        }
    }
}