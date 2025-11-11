using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class IntroDebug : MonoBehaviour
{
    public PlayableDirector timeline;
    public GameObject player;
    public GameObject playerCam;
    public GameObject[] canvases;

    void Awake()
    {
        Debug.Log("[IntroDebug] Awake scene=" + SceneManager.GetActiveScene().name);
        if (player == null) Debug.Log("[IntroDebug] player IS NULL");
        else Debug.Log("[IntroDebug] player=" + player.name);

        if (playerCam == null) Debug.Log("[IntroDebug] playerCam IS NULL");
        else Debug.Log("[IntroDebug] playerCam=" + playerCam.name);

        if (canvases == null) Debug.Log("[IntroDebug] canvases IS NULL or empty");
        else Debug.Log("[IntroDebug] canvases count=" + canvases.Length + " -> " + string.Join(", ", System.Array.ConvertAll(canvases, g => g?.name ?? "null")));
    }

    void Start()
    {
        Debug.Log("[IntroDebug] Start. Timeline assigned? " + (timeline != null && timeline.playableAsset != null));
        if (timeline != null)
        {
            Debug.Log("[IntroDebug] timeline.duration=" + timeline.duration);
        }
    }

    // poner esto para ver si el stopped se dispara
    void OnEnable()
    {
        if (timeline != null) timeline.stopped += TimelineStopped;
    }
    void OnDisable()
    {
        if (timeline != null) timeline.stopped -= TimelineStopped;
    }

    void TimelineStopped(PlayableDirector pd)
    {
        Debug.Log("[IntroDebug] PlayableDirector stopped event fired for " + pd.gameObject.name);
    }
}
