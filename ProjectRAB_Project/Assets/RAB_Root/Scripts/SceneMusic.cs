using UnityEngine;

public class SceneMusic : MonoBehaviour
{
    [Header("Música de esta escena")]
    public AudioClip musicClip; // Clip específico para la escena
    public bool loop = true;    // ¿Repetir música?

    void Start()
    {
        if (PersistentAudioManager.Instance != null && musicClip != null)
        {
            PersistentAudioManager.Instance.PlayMusic(musicClip, loop);
        }
    }
}
