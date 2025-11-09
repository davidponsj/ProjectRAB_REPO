using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroVideoController : MonoBehaviour
{
    [Header("Video & Escena")]
    public VideoPlayer videoPlayer;
    public string nextSceneName = "Menu";

    [Header("Fade Out")]
    public CanvasGroup fadeCanvas;    // un CanvasGroup negro que cubre la pantalla
    public float fadeDuration = 0.5f; // duración del fade

    [Header("Skip Input")]
    public KeyCode[] skipKeys = new KeyCode[] { KeyCode.Space, KeyCode.Escape, KeyCode.Return };
    public string[] skipButtons = new string[] { "Submit" }; // Input Manager para mando

    private bool isSkipping = false;
    private bool videoStarted = false;

    void Start()
    {
        isSkipping = false;

        if (videoPlayer == null || videoPlayer.clip == null)
        {
            Debug.LogError("VideoPlayer no tiene clip asignado!");
            return;
        }

        // Asegurarse de que el fadeCanvas esté invisible
        if (fadeCanvas != null) fadeCanvas.alpha = 0f;

        // Comenzar a reproducir
        videoPlayer.Play();

        // Iniciamos corutina para esperar que el video realmente comience
        StartCoroutine(WaitForVideoStart());
    }

    IEnumerator WaitForVideoStart()
    {
        // Espera mientras el video no esté reproduciéndose
        while (!videoPlayer.isPlaying)
            yield return null;

        videoStarted = true;

        // Conectamos el evento de fin de video
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void Update()
    {
        // Solo permite saltar después de que el video haya empezado
        if (videoStarted && !isSkipping && (AnySkipKeyPressed() || AnySkipButtonPressed()))
        {
            StartCoroutine(SkipVideo());
        }
    }

    IEnumerator SkipVideo()
    {
        isSkipping = true;

        // Fade out si hay CanvasGroup asignado
        if (fadeCanvas != null)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                t += Time.deltaTime;
                fadeCanvas.alpha = Mathf.Clamp01(t / fadeDuration);
                yield return null;
            }
            fadeCanvas.alpha = 1f;
        }

        SceneManager.LoadScene(nextSceneName);
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        // Solo dispara si no se está saltando
        if (!isSkipping)
        {
            StartCoroutine(SkipVideo());
        }
    }

    // Funciones auxiliares para detectar input
    bool AnySkipKeyPressed()
    {
        foreach (var k in skipKeys)
            if (Input.GetKeyDown(k)) return true;
        return false;
    }

    bool AnySkipButtonPressed()
    {
        foreach (var b in skipButtons)
            if (!string.IsNullOrEmpty(b) && Input.GetButtonDown(b))
                return true;
        return false;
    }
}