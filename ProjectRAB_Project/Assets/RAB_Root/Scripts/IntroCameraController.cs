using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class IntroCameraController : MonoBehaviour
{
    [Header("Timeline y cámaras")]
    public PlayableDirector timeline;    // Timeline de la cámara de intro
    public GameObject player;            // Jugador principal
    public GameObject playerCam;         // Cámara del jugador (si usa Cinemachine)

    [Header("HUD / Canvas")]
    public GameObject[] canvases;        // Todos los Canvas del HUD a ocultar/mostrar

    private bool hasEnded = false;       // Control interno para no ejecutar dos veces

    void Start()
    {
        Debug.Log($"[Intro] Start en escena: {SceneManager.GetActiveScene().name}");

        // Si ya se ha reproducido la intro en esta sesión, saltamos la animación
        if (IntroManager.hasPlayedIntro)
        {
            Debug.Log("[Intro] Ya se ha reproducido, saltando animación.");
            ActivateGameplay();
            return;
        }

        // Primera vez: ocultamos HUD y jugador
        foreach (var c in canvases) HideCanvas(c);
        if (player != null) player.SetActive(false);
        if (playerCam != null) playerCam.SetActive(false);

        // Reproducir la animación
        if (timeline != null && timeline.playableAsset != null)
        {
            timeline.time = 0;
            timeline.Evaluate();
            timeline.Play();

            // Suscribirse al evento de finalización
            timeline.stopped += OnTimelineFinished;

            // Seguridad: si el evento no salta, forzamos activación tras duración + margen
            StartCoroutine(ForceFinishAfterTimeline());
        }
        else
        {
            Debug.LogWarning("[Intro] No hay Timeline asignada o PlayableAsset vacío. Activando gameplay directamente.");
            ActivateGameplay();
        }

        // Marcamos que ya se ha reproducido
        IntroManager.hasPlayedIntro = true;
    }

    private IEnumerator ForceFinishAfterTimeline()
    {
        yield return new WaitForSeconds((float)(timeline != null ? timeline.duration : 3f) + 0.2f);

        if (!hasEnded)
        {
            Debug.LogWarning("[Intro] Evento 'stopped' no disparado, forzando finalización manual.");
            OnTimelineFinished(timeline);
        }
    }

    private void OnTimelineFinished(PlayableDirector pd)
    {
        if (hasEnded) return; // evitar doble ejecución
        hasEnded = true;

        Debug.Log("[Intro] Timeline terminada, activando HUD y Player.");

        ActivateGameplay();
    }

    private void ActivateGameplay()
    {
        foreach (var c in canvases) ShowCanvas(c);
        if (player != null) player.SetActive(true);
        if (playerCam != null) playerCam.SetActive(true);
        if (timeline != null) timeline.gameObject.SetActive(false);
    }

    private void HideCanvas(GameObject canvas)
    {
        if (canvas == null) return;
        CanvasGroup cg = canvas.GetComponent<CanvasGroup>();
        if (cg == null) cg = canvas.AddComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    private void ShowCanvas(GameObject canvas)
    {
        if (canvas == null) return;
        CanvasGroup cg = canvas.GetComponent<CanvasGroup>();
        if (cg == null) cg = canvas.AddComponent<CanvasGroup>();
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }
}