using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class IntroCameraController_Debug : MonoBehaviour
{
    [Header("Asignar en Inspector (si no, se buscan por tag/nombre)")]
    public PlayableDirector timeline;
    public GameObject player;
    public GameObject playerCam;
    public GameObject[] canvases;

    bool finished = false;

    void Awake()
    {
        // Búsqueda defensiva de referencias
        if (player == null)
        {
            player = GameObject.FindWithTag("Player");
            Debug.Log("[IntroDebug] Awake: buscado player -> " + (player ? player.name : "NULL"));
        }
        if (playerCam == null)
        {
            playerCam = GameObject.FindWithTag("PlayerCam");
            Debug.Log("[IntroDebug] Awake: buscado playerCam -> " + (playerCam ? playerCam.name : "NULL"));
        }
        if (canvases == null || canvases.Length == 0)
        {
            var found = GameObject.FindObjectsOfType<Canvas>().Select(c => c.gameObject).ToArray();
            canvases = found;
            Debug.Log("[IntroDebug] Awake: canvases encontrados -> count=" + canvases.Length + " -> " + string.Join(", ", canvases.Select(g => g.name)));
        }
    }

    IEnumerator Start()
    {
        Debug.Log("[IntroDebug] Start escena=" + SceneManager.GetActiveScene().name);

        // detectar otros PlayableDirectors que puedan interferir (por DontDestroyOnLoad)
        var allDirs = FindObjectsOfType<PlayableDirector>();
        Debug.Log("[IntroDebug] PlayableDirectors en jerarquía: " + string.Join(", ", allDirs.Select(d => d.gameObject.name + (d == timeline ? " (THIS)" : ""))));
        foreach (var d in allDirs)
        {
            if (d != timeline)
            {
                Debug.Log("[IntroDebug] Desactivando temporalmente PlayableDirector de: " + d.gameObject.name);
                d.enabled = false;
            }
        }

        // Si ya se ha reproducido en esta sesión (si usas IntroManager)
        if (IntroManager.hasPlayedIntro)
        {
            Debug.Log("[IntroDebug] IntroManager.hasPlayedIntro == true -> activar gameplay inmediatamente");
            ActivateGameplay();
            yield break;
        }

        // Oculta HUD y jugador
        foreach (var c in canvases) HideCanvas(c);
        if (player != null) player.SetActive(false);
        if (playerCam != null) playerCam.SetActive(false);

        // timeline defensivo
        if (timeline == null)
        {
            Debug.LogError("[IntroDebug] timeline NO asignado en el inspector. Cancelando intro.");
            ActivateGameplay();
            yield break;
        }

        if (timeline.playableAsset == null)
        {
            Debug.LogError("[IntroDebug] timeline.playableAsset es NULL. Cancelando intro.");
            ActivateGameplay();
            yield break;
        }

        // Suscribirse ANTES de Play para no perdernos el evento
        timeline.stopped += OnTimelineStopped;
        Debug.Log("[IntroDebug] Suscrito a stopped.");

        // Reiniciar de forma segura y reproducir
        Debug.Log("[IntroDebug] timeline.duration = " + timeline.duration);
        timeline.Stop();
        timeline.time = 0;
        timeline.Evaluate();
        Debug.Log("[IntroDebug] timeline reiniciado y evaluado (time= " + timeline.time + ")");

        // Espera un frame para asegurarnos que todo está listo (útil al venir de otra escena)
        yield return null;

        timeline.Play();
        Debug.Log("[IntroDebug] timeline.Play() llamado.");

        // seguridad: espera la duración real + margen y si no terminó forzamos
        StartCoroutine(ForceFinishAfter((float)timeline.duration + 0.3f));
    }

    IEnumerator ForceFinishAfter(float wait)
    {
        yield return new WaitForSeconds(wait);
        if (!finished)
        {
            Debug.LogWarning("[IntroDebug] ForceFinishAfter: timeline no finalizó por evento, forzando OnTimelineStopped.");
            OnTimelineStopped(timeline);
        }
    }

    void OnTimelineStopped(PlayableDirector pd)
    {
        if (finished) return;
        finished = true;
        Debug.Log("[IntroDebug] OnTimelineStopped invocado por: " + (pd ? pd.gameObject.name : "NULL"));
        ActivateGameplay();
    }

    void ActivateGameplay()
    {
        Debug.Log("[IntroDebug] Activando HUD y player.");
        foreach (var c in canvases) ShowCanvas(c);
        if (player != null) { player.SetActive(true); Debug.Log("[IntroDebug] Player activado: " + player.name); }
        else Debug.LogWarning("[IntroDebug] Player es NULL al activar.");

        if (playerCam != null) { playerCam.SetActive(true); Debug.Log("[IntroDebug] playerCam activada: " + playerCam.name); }
        else Debug.LogWarning("[IntroDebug] playerCam es NULL al activar.");

        if (timeline != null) timeline.gameObject.SetActive(false);

        // Reactivar temporalmente otros PlayableDirectors (si era necesario)
        var allDirs = FindObjectsOfType<PlayableDirector>();
        foreach (var d in allDirs)
        {
            if (d != timeline && !d.enabled)
            {
                d.enabled = true;
                Debug.Log("[IntroDebug] Reactivado PlayableDirector: " + d.gameObject.name);
            }
        }
    }

    void HideCanvas(GameObject canvas)
    {
        if (canvas == null) return;
        var cg = canvas.GetComponent<CanvasGroup>();
        if (cg == null) cg = canvas.AddComponent<CanvasGroup>();
        cg.alpha = 0; cg.interactable = false; cg.blocksRaycasts = false;
        Debug.Log("[IntroDebug] HideCanvas: " + canvas.name);
    }

    void ShowCanvas(GameObject canvas)
    {
        if (canvas == null) return;
        var cg = canvas.GetComponent<CanvasGroup>();
        if (cg == null) cg = canvas.AddComponent<CanvasGroup>();
        cg.alpha = 1; cg.interactable = true; cg.blocksRaycasts = true;
        Debug.Log("[IntroDebug] ShowCanvas: " + canvas.name);
    }

    // Debug helper para ver si el objeto persistente de sonido tiene componentes "peligrosos"
    [ContextMenu("CheckPersistentAudioObject")]
    void CheckPersistentAudio()
    {
        var objs = Resources.FindObjectsOfTypeAll<GameObject>().Where(go => go.name.ToLower().Contains("audio") || go.name.ToLower().Contains("sound"));
        foreach (var g in objs)
        {
            Debug.Log("[IntroDebug] Posible audio object: " + g.name + " components: " + string.Join(", ", g.GetComponents<Component>().Select(c => c.GetType().Name)));
        }
    }
}