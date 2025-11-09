using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MenuButtonSFX : MonoBehaviour
{
    public AudioClip clickSound; // arrastra aquí el clip del botón

    void Start()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() =>
        {
            if (PersistentAudioManager.Instance != null && clickSound != null)
                PersistentAudioManager.Instance.PlaySFX(clickSound);
        });
    }
}
