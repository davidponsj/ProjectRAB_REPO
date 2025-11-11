using UnityEngine;
using UnityEngine.Playables;

public class IntroCameraController : MonoBehaviour
{
    [Header("Timeline y cámaras")]
    public PlayableDirector timeline;    // Timeline de la cámara de intro
    public GameObject player;            // jugador
    public GameObject playerCam;         // cámara virtual del jugador (si usa Cinemachine)

    [Header("HUD / Canvas")]
    public GameObject[] canvases;        // todos los Canvas del HUD

    void Start()
    {
        // Si la intro ya se reprodujo, activamos todo y no reproducimos animación
        if (IntroManager.hasPlayedIntro)
        {
            foreach (var c in canvases) ShowCanvas(c);
            player.SetActive(true);
            if (playerCam != null) playerCam.SetActive(true);
            timeline.gameObject.SetActive(false); // Timeline desactivado
            return;
        }

        // Primera vez: desactivamos HUD y jugador
        foreach (var c in canvases) HideCanvas(c);
        player.SetActive(false);
        if (playerCam != null) playerCam.SetActive(false);

        // Reproducimos la animación de la cámara
        timeline.Play();

        // Marcamos que ya se reprodujo
        IntroManager.hasPlayedIntro = true;

        // Conectamos el evento de final de animación
        timeline.stopped += OnTimelineFinished;
    }

    void OnTimelineFinished(PlayableDirector pd)
    {
        // Activar HUD / Canvas
        foreach (var c in canvases) ShowCanvas(c);

        // Activar jugador
        player.SetActive(true);

        // Desactivar Timeline / cámara de intro
        timeline.gameObject.SetActive(false);

        // Activar cámara del jugador
        if (playerCam != null) playerCam.SetActive(true);
    }

    // Función para ocultar canvas (visual + interacción)
    void HideCanvas(GameObject canvas)
    {
        CanvasGroup cg = canvas.GetComponent<CanvasGroup>();
        if (cg == null) cg = canvas.AddComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    // Función para mostrar canvas (visual + interacción)
    void ShowCanvas(GameObject canvas)
    {
        CanvasGroup cg = canvas.GetComponent<CanvasGroup>();
        if (cg == null) cg = canvas.AddComponent<CanvasGroup>();
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }
}
