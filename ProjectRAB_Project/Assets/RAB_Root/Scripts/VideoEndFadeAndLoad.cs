using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections;

public class VideoEndFadeAndLoad : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public Image fadeImage;
    public string nextSceneName;
    public float fadeDuration = 1.5f;

    bool fadeStarted = false;
    bool sceneLoaded = false;

    void Start()
    {
        // Si quieres mantener la seguridad por si el vídeo llega a su fin "antes",
        // dejamos el event handler, pero protegemos con flags para que no se ejecute dos veces.
        videoPlayer.loopPointReached += OnVideoFinished;

        StartCoroutine(MonitorVideoForFade());
    }

    IEnumerator MonitorVideoForFade()
    {
        // Esperamos hasta que haya información válida de duración del vídeo
        // (por ejemplo si el clip se está cargando).
        float videoLength = 0f;
        // Si estamos usando clip directo:
        if (videoPlayer.clip != null)
        {
            videoLength = (float)videoPlayer.clip.length;
        }
        else
        {
            // Si no hay clip asignado (por streaming), intentamos usar videoPlayer.length
            // y esperamos hasta que sea mayor que 0.
            while (videoPlayer.length <= 0.0)
            {
                yield return null;
            }
            videoLength = (float)videoPlayer.length;
        }

        // Calculamos el tiempo en que debe empezar el fade.
        float fadeStartTime = videoLength - fadeDuration;
        if (fadeStartTime < 0f) fadeStartTime = 0f; // si el fade es más largo que el vídeo, empieza desde 0

        // Si el vídeo no ha empezado a reproducirse aún, esperamos a que lo haga.
        while (!videoPlayer.isPlaying)
        {
            // Si no quieres esperar a que empiece (por ejemplo el video ya está en un frame),
            // puedes comentar este while y usar la comprobación del tiempo directamente.
            yield return null;
        }

        // Monitoreamos el tiempo de reproducción y lanzamos el fade cuando corresponda.
        while (!fadeStarted && videoPlayer.isPlaying)
        {
            // videoPlayer.time puede ser double; lo convertimos a float para comparar
            if ((float)videoPlayer.time >= fadeStartTime)
            {
                StartCoroutine(FadeOutAndChangeScene());
                fadeStarted = true;
                break;
            }
            yield return null;
        }

        // Si terminara el bucle sin empezar el fade (p.ej. video deja de estar isPlaying
        // por cualquier motivo), no hacemos nada aquí; el event loopPointReached
        // se encargará de terminar la escena (ver OnVideoFinished).
    }

    void OnVideoFinished(VideoPlayer vp)
    {
        // Si aún no hemos empezado el fade, lo forzamos ahora y nos aseguramos
        // de no hacer doble carga.
        if (!fadeStarted)
        {
            StartCoroutine(FadeOutAndChangeScene());
            fadeStarted = true;
        }
    }

    IEnumerator FadeOutAndChangeScene()
    {
        if (sceneLoaded) yield break; // protección contra dobles llamadas

        // Asegúrate de que la imagen y su alpha están en estado inicial correcto
        if (fadeImage == null)
        {
            Debug.LogWarning("Fade Image no asignada.");
            yield break;
        }

        Color color = fadeImage.color;
        // forzamos alpha a 0 por si acaso (visible)
        color.a = Mathf.Clamp01(color.a);
        fadeImage.color = color;

        float time = 0f;
        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float a = Mathf.Lerp(0f, 1f, time / fadeDuration);
            color.a = a;
            fadeImage.color = color;
            yield return null;
        }
        // asegura alpha final
        color.a = 1f;
        fadeImage.color = color;

        // Cargamos la siguiente escena (si se ha puesto nombre)
        sceneLoaded = true;
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    void OnDestroy()
    {
        // limpiar suscripción
        videoPlayer.loopPointReached -= OnVideoFinished;
    }
}
