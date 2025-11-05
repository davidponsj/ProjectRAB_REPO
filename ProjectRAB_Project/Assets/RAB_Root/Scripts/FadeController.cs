using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class FadeController : MonoBehaviour
{
    public static FadeController Instance { get; private set; }

    [Tooltip("Image que cubre la pantalla (negra).")]
    public Image fadeImage;
    [Tooltip("Duración por defecto del fundido en segundos.")]
    public float defaultDuration = 0.8f;

    // Flags para controlar si debemos hacer fade-in cuando la escena termina de cargar
    private bool expectFadeIn = false;
    private float expectedFadeInDuration = 0.8f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;

            // Asegurar alpha inicial transparente
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = 0f;
                fadeImage.color = c;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Se ejecuta cuando termina de cargar una escena
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (expectFadeIn && fadeImage != null)
        {
            // resetear flag para que no haga fade-in en cargas posteriores no iniciadas por este controller
            expectFadeIn = false;
            StartCoroutine(Fade(1f, 0f, expectedFadeInDuration));
        }
        else
        {
            // asegurar transparencia si no hacemos fade-in
            if (fadeImage != null)
            {
                Color c = fadeImage.color;
                c.a = 0f;
                fadeImage.color = c;
            }
        }
    }

    // Llamar para hacer fade-out, cargar asincrónicamente y luego hacer fade-in en la nueva escena
    public void FadeOutLoadAndIn(string sceneName, float duration = -1f)
    {
        if (duration <= 0) duration = defaultDuration;
        StartCoroutine(FadeOutLoadAndInCoroutine(sceneName, duration));
    }

    private IEnumerator FadeOutLoadAndInCoroutine(string sceneName, float duration)
    {
        // Fade out (transparente -> negro)
        if (fadeImage != null)
            yield return StartCoroutine(Fade(0f, 1f, duration));

        // Preparamos el fade-in al cargar la escena
        expectFadeIn = true;
        expectedFadeInDuration = duration;

        // Cargar asincrónicamente
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        if (op == null)
        {
            Debug.LogError("[FadeController] LoadSceneAsync devolvió null. Revisa el nombre de la escena y Build Settings.");
            yield break;
        }

        // Esperamos hasta que esté listo (op.progress alcanza 0.9)
        while (op.progress < 0.9f)
            yield return null;

        // Pequeña espera opcional para asegurar que el frame donde la escena se activa es correcto
        yield return new WaitForSeconds(0.02f);

        // Activar la escena (esto disparará OnSceneLoaded y hará el fade-in)
        op.allowSceneActivation = true;

        // Esperamos hasta que termine completamente (op.isDone será true)
        while (!op.isDone)
            yield return null;
    }

    // Corrutina genérica de fundido (interpola alpha)
    private IEnumerator Fade(float fromAlpha, float toAlpha, float duration)
    {
        if (fadeImage == null) yield break;

        float t = 0f;
        Color c = fadeImage.color;
        c.a = fromAlpha;
        fadeImage.color = c;

        while (t < duration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(fromAlpha, toAlpha, t / duration);
            c.a = a;
            fadeImage.color = c;
            yield return null;
        }

        c.a = toAlpha;
        fadeImage.color = c;
    }
}
