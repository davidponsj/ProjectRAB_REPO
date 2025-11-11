using UnityEngine;
using UnityEngine.Playables;
using System.Linq;
using UnityEngine.SceneManagement;

public class IntroCameraController : MonoBehaviour
{
    [Header("Timeline y cámaras")]
    public PlayableDirector timeline;    // Timeline de la cámara de intro
    public GameObject player;            // jugador
    public GameObject playerCam;         // cámara virtual del jugador (Cinemachine)

    [Header("HUD / Canvas")]
    public GameObject[] canvases;        // todos los Canvas del HUD

    void Awake()
    {
        // Buscar referencias automáticamente si no están asignadas
        if (player == null)
            player = GameObject.FindWithTag("Player");

        if (playerCam == null)
            playerCam = GameObject.FindWithTag("PlayerCam");

        if (canvases == null || canvases.Length == 0)
            canvases = GameObject.FindObjectsOfType<Canvas>()
                       .Select(c => c.gameObject).ToArray();
    }

    void Start()
    {
        string key = "IntroPlayed_" + SceneManager.GetActiveScene().name;

        // Si la intro ya se reprodujo, activamos todo y salimos
        if (PlayerPrefs.GetInt(key, 0) == 1)
        {
            foreach (var c in canvases) ShowCanvas(c);
            if (player != null) player.SetActive(true);
            if (playerCam != null) playerCam.SetActive(true);
            if (timeline != null) timeline.gameObject.SetActive(false);
            return;
        }

        // Primera vez: ocultamos HUD y jugador
        foreach (var c in canvases) HideCanvas(c);
        if (player != null) player.SetActive(false);
        if (playerCam != null) playerCam.SetActive(false);

        // Reproducimos la animación de la cámara
        if (timeline != null)
        {
            timeline.Play();
            timeline.stopped += OnTimelineFinished;
        }

        // Marcamos que ya se reprodujo
        PlayerPrefs.SetInt(key, 1);
    }

    void OnTimelineFinished(PlayableDirector pd)
    {
        // Activar HUD / Canvas
        foreach (var c in canvases) ShowCanvas(c);

        // Activar jugador
        if (player != null) player.SetActive(true);

        // Desactivar Timeline / cámara de intro
        if (timeline != null) timeline.gameObject.SetActive(false);

        // Activar cámara del jugador
        if (playerCam != null) playerCam.SetActive(true);
    }

    void HideCanvas(GameObject canvas)
    {
        CanvasGroup cg = canvas.GetComponent<CanvasGroup>();
        if (cg == null) cg = canvas.AddComponent<CanvasGroup>();
        cg.alpha = 0;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    void ShowCanvas(GameObject canvas)
    {
        CanvasGroup cg = canvas.GetComponent<CanvasGroup>();
        if (cg == null) cg = canvas.AddComponent<CanvasGroup>();
        cg.alpha = 1;
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }
}

