using UnityEngine;
using UnityEngine.UI;

public class AudioSettingsSimple : MonoBehaviour
{
    [Header("Referencias")]
    public AudioSource musicSource; // arrastra MusicSource aquí
    public AudioSource sfxSource;   // arrastra SFXPlayer -> AudioSource aquí

    [Header("Sliders (UI)")]
    public Slider musicSlider;      // arrastra Slider_Music
    public Slider sfxSlider;        // arrastra Slider_SFX

    const string MUSIC_KEY = "MusicVolSimple";
    const string SFX_KEY = "SFXVolSimple";

    void Start()
    {
        // Cargar valores guardados (si existen), por defecto 1
        float mv = PlayerPrefs.HasKey(MUSIC_KEY) ? PlayerPrefs.GetFloat(MUSIC_KEY) : 1f;
        float sv = PlayerPrefs.HasKey(SFX_KEY) ? PlayerPrefs.GetFloat(SFX_KEY) : 1f;

        if (musicSlider != null) musicSlider.value = mv;
        if (sfxSlider != null) sfxSlider.value = sv;

        ApplyMusic(mv);
        ApplySFX(sv);

        // Añadir listeners a sliders (si no los vas a conectar por Inspector)
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
    }

    public void OnMusicSliderChanged(float v)
    {
        ApplyMusic(v);
        PlayerPrefs.SetFloat(MUSIC_KEY, v);
    }

    public void OnSFXSliderChanged(float v)
    {
        ApplySFX(v);
        PlayerPrefs.SetFloat(SFX_KEY, v);
    }

    void ApplyMusic(float linear)
    {
        if (musicSource != null) musicSource.volume = Mathf.Clamp01(linear);
    }

    void ApplySFX(float linear)
    {
        if (sfxSource != null) sfxSource.volume = Mathf.Clamp01(linear);
    }
}
