using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroVideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;  // arrastra aquí tu componente VideoPlayer
    public string nextSceneName = "Menu"; // la escena a la que saltará

    private bool isSkipping = false;

    void Start()
    {
        // Cuando el video termina, llamamos a OnVideoEnd
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void Update()
    {
        // Si pulsa una tecla o botón, saltamos
        if (!isSkipping && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape)))
        {
            SkipVideo();
        }
    }

    void SkipVideo()
    {
        isSkipping = true;
        // Opcional: aquí puedes llamar a tu función de fade out
        // StartCoroutine(FadeOutAndLoad());
        SceneManager.LoadScene(nextSceneName);
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        if (!isSkipping)
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}