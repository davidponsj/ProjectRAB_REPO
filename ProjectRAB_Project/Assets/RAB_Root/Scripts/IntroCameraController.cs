using UnityEngine;
using UnityEngine.Playables;

public class IntroCameraController : MonoBehaviour
{
    public PlayableDirector timeline;  // tu Timeline que reproduce la cámara
    public GameObject[] canvases;      // todos los canvas del HUD
    public GameObject player;          // jugador

    void Start()
    {
        // Si la intro ya se reprodujo, activamos todo y no reproducimos animación
        if (IntroManager.hasPlayedIntro)
        {
            foreach (var c in canvases) c.SetActive(true);
            player.SetActive(true);
            timeline.gameObject.SetActive(false); // Timeline desactivado
            return;
        }

        // Primera vez: desactivamos todo y reproducimos la animación
        foreach (var c in canvases) c.SetActive(false);
        player.SetActive(false);

        timeline.Play();

        // Marcamos que ya se reprodujo
        IntroManager.hasPlayedIntro = true;

        // Llamamos a activar todo cuando la animación termina
        timeline.stopped += OnTimelineFinished;
    }

    void OnTimelineFinished(PlayableDirector pd)
    {
        foreach (var c in canvases) c.SetActive(true);
        player.SetActive(true);
    }
}