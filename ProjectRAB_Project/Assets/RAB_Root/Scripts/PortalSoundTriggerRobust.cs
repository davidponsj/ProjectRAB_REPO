using UnityEngine;

public class PortalFadeTrigger : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private float fadeDuration = 0.8f;
    [SerializeField] private bool requireTagPlayer = true;

    private void OnTriggerEnter(Collider other)
    {
        if (requireTagPlayer && !other.CompareTag("Player")) return;

        // Opcional: desactivar control del jugador si tienes un script PlayerController
        // var pc = other.GetComponent<PlayerController>();
        // if (pc != null) pc.enabled = false;

        if (FadeController.Instance != null)
            FadeController.Instance.FadeOutLoadAndIn(sceneToLoad, fadeDuration);
        else
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
}
