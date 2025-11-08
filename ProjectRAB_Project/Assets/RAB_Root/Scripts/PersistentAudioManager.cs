using System;
using UnityEngine;
using UnityEngine.UI;

public class PersistentAudioManager : MonoBehaviour
{
    public static PersistentAudioManager Instance;

    [Header("AudioSources")]
    public AudioSource musicSource; // asigna en inspector
    public AudioSource sfxSource;   // asigna en inspector

    [Header("UI Sliders (opcional)")]
    public Slider musicSlider;
    public Slider sfxSlider;

    const string MUSIC_KEY = "MusicVolSimple";
    const string SFX_KEY = "SFXVolSimple";

    // Estado actual (0..1)
    public float CurrentMusicVolume { get; private set; } = 1f;
    public float CurrentSFXVolume { get; private set; } = 1f;

    // Evento para notificar cambios de volumen (sfx o music)
    public event Action<float> OnSFXVolumeChanged;
    public event Action<float> OnMusicVolumeChanged;

    void Awake()
    {
        // Singleton básico
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

        // Cargar valores guardados y aplicarlos
        CurrentMusicVolume = PlayerPrefs.HasKey(MUSIC_KEY) ? PlayerPrefs.GetFloat(MUSIC_KEY) : 1f;
        CurrentSFXVolume = PlayerPrefs.HasKey(SFX_KEY) ? PlayerPrefs.GetFloat(SFX_KEY) : 1f;

        ApplyMusic(CurrentMusicVolume);
        ApplySFX(CurrentSFXVolume);

        // Inicializar sliders (si están referenciados)
        if (musicSlider != null) musicSlider.value = CurrentMusicVolume;
        if (sfxSlider != null) sfxSlider.value = CurrentSFXVolume;

        // Añadir listeners si los sliders existen
        if (musicSlider != null) musicSlider.onValueChanged.AddListener(OnMusicSliderChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.AddListener(OnSFXSliderChanged);
    }

    public void OnMusicSliderChanged(float v)
    {
        SetMusicVolume(v);
    }

    public void OnSFXSliderChanged(float v)
    {
        SetSFXVolume(v);
    }

    // Método público por si quieres cambiar volumen desde otros scripts
    public void SetMusicVolume(float linear)
    {
        CurrentMusicVolume = Mathf.Clamp01(linear);
        ApplyMusic(CurrentMusicVolume);
        PlayerPrefs.SetFloat(MUSIC_KEY, CurrentMusicVolume);
        OnMusicVolumeChanged?.Invoke(CurrentMusicVolume);
    }

    public void SetSFXVolume(float linear)
    {
        CurrentSFXVolume = Mathf.Clamp01(linear);
        ApplySFX(CurrentSFXVolume);
        PlayerPrefs.SetFloat(SFX_KEY, CurrentSFXVolume);
        OnSFXVolumeChanged?.Invoke(CurrentSFXVolume);
    }

    void ApplyMusic(float linear)
    {
        if (musicSource != null) musicSource.volume = Mathf.Clamp01(linear);
    }

    void ApplySFX(float linear)
    {
        if (sfxSource != null) sfxSource.volume = Mathf.Clamp01(linear);
    }

    // Reproducir SFX vía el SFXPlayer central
    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null || sfxSource == null) return;
        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume * CurrentSFXVolume));
    }

    // Cambiar la música actual
    public void PlayMusicClip(AudioClip clip, bool loop = true)
    {
        if (musicSource == null) return;
        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.Play();
        ApplyMusic(CurrentMusicVolume);
    }
}
