using UnityEngine;
using UnityEngine.UI;

public class SceneAudioUI : MonoBehaviour
{
    public Slider musicSlider;
    public Slider sfxSlider;

    void Start()
    {
        if (PersistentAudioManager.Instance == null)
        {
            Debug.LogError("PersistentAudioManager no encontrado.");
            return;
        }

        // Conectar sliders a AudioManager
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
        if (PersistentAudioManager.Instance != null)
        {
            if (musicSlider != null)
                musicSlider.onValueChanged.RemoveListener(PersistentAudioManager.Instance.SetMusicVolume);
            if (sfxSlider != null)
                sfxSlider.onValueChanged.RemoveListener(PersistentAudioManager.Instance.SetSFXVolume);
        }
    }
}
