using UnityEngine;
using UnityEngine.SceneManagement;

public class PersistentAudioManager : MonoBehaviour
{
    public static PersistentAudioManager Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Range(0f, 1f)]
    public float CurrentMusicVolume = 1f;
    [Range(0f, 1f)]
    public float CurrentSFXVolume = 1f;

    public AudioClip menuMusic; // Música específica del menú

    void Awake()
    {
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

        ApplyVolumes();
        SceneManager.sceneLoaded += OnSceneLoaded; // Nos avisará cuando cambie la escena
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Ejemplo: reproducir música solo en la escena "Menu"
        if (scene.name == "SCN_MainMenuTest")
        {
            PlayMusic(menuMusic, true);
        }
        else
        {
            // Para otras escenas, para la música
            if (musicSource.isPlaying)
                musicSource.Stop();
        }
    }

    public void SetMusicVolume(float vol)
    {
        CurrentMusicVolume = Mathf.Clamp01(vol);
        ApplyVolumes();
    }

    public void SetSFXVolume(float vol)
    {
        CurrentSFXVolume = Mathf.Clamp01(vol);
        ApplyVolumes();
    }

    private void ApplyVolumes()
    {
        if (musicSource != null)
            musicSource.volume = CurrentMusicVolume;
        if (sfxSource != null)
            sfxSource.volume = CurrentSFXVolume;
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip, CurrentSFXVolume);
    }

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