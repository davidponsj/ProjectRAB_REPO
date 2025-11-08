using System.Collections;
using UnityEngine;
using TMPro;                   // TextMeshPro
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TypewriterSkipController : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI textUI;     // arrastra aquí tu TextMeshProUGUI
    [TextArea(3, 10)]
    public string fullText;            // texto completo (puedes also setearlo desde inspector)
    public Button continueButton;      // arrastra aquí el botón que llevará al tutorial

    [Header("Typewriter")]
    public float charDelay = 0.03f;    // tiempo entre caracteres
    public bool playOnStart = true;

    [Header("Input para saltar")]
    public KeyCode[] skipKeys = new KeyCode[] { KeyCode.Space, KeyCode.Escape, KeyCode.Return };
    public string[] skipButtons = new string[] { "Submit" }; // input axis/btn (opcional, para mando)

    [Header("Escena")]
    public string tutorialSceneName = "Tutorial"; // nombre exacto de la escena a cargar

    bool isTyping = false;
    Coroutine typingCoroutine;

    void Start()
    {
        if (continueButton != null)
            continueButton.gameObject.SetActive(false);

        if (textUI == null)
        {
            Debug.LogError("TypewriterSkipController: asigna TextMeshProUGUI en el inspector.");
            enabled = false;
            return;
        }

        textUI.text = "";

        if (playOnStart)
            StartTyping();
    }

    void Update()
    {
        // detectar pulsación para saltar
        if (isTyping && (AnySkipKeyPressed() || AnySkipButtonPressed()))
        {
            // salto: mostrar texto completo ya
            SkipTyping();
        }
    }

    // Inicia el typewriter (puedes llamarlo desde otro script)
    public void StartTyping()
    {
        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeTextCoroutine());
    }

    IEnumerator TypeTextCoroutine()
    {
        isTyping = true;
        textUI.text = "";
        continueButton?.gameObject.SetActive(false);

        for (int i = 0; i < fullText.Length; i++)
        {
            textUI.text += fullText[i];
            yield return new WaitForSeconds(charDelay);
        }

        OnTypingComplete();
    }

    void SkipTyping()
    {
        if (!isTyping) return;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        textUI.text = fullText;
        OnTypingComplete();
    }

    void OnTypingComplete()
    {
        isTyping = false;
        typingCoroutine = null;

        if (continueButton != null)
        {
            continueButton.gameObject.SetActive(true);
            // quitar listeners previos por seguridad
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(OnContinuePressed);
        }
    }

    void OnContinuePressed()
    {
        // Aquí cargas tu escena del tutorial (puedes añadir fade si quieres)
        if (!string.IsNullOrEmpty(tutorialSceneName))
            SceneManager.LoadScene(tutorialSceneName);
    }

    bool AnySkipKeyPressed()
    {
        foreach (var k in skipKeys)
            if (Input.GetKeyDown(k)) return true;
        return false;
    }

    bool AnySkipButtonPressed()
    {
        // útil si usas Input Manager y botones definidos (ej: "Submit" para mando)
        foreach (var b in skipButtons)
        {
            if (!string.IsNullOrEmpty(b) && Input.GetButtonDown(b))
                return true;
        }
        return false;
    }
}