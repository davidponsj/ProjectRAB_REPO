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
    public string fullText;            // texto completo (puedes también setearlo desde inspector)
    public Button continueButton;      // arrastra aquí el botón que llevará al tutorial

    [Header("Typewriter")]
    public float charDelay = 0.03f;    // tiempo entre caracteres
    public bool playOnStart = true;

    [Header("Input para saltar")]
    public KeyCode[] skipKeys = new KeyCode[] { KeyCode.Space, KeyCode.Escape, KeyCode.Return };
    public string[] skipButtons = new string[] { "Submit" }; // input axis/btn (opcional, para mando)

    [Header("Escena")]
    public string tutorialSceneName = "Tutorial"; // nombre exacto de la escena a cargar

    [Header("Sonido de tecleo")]
    public AudioSource audioSource;                // arrastra aquí el componente AudioSource (Play On Awake desactivado)
    public AudioClip typeSound;                    // arrastra aquí el clip de sonido de tecla (un click corto)
    public bool soundPerWord = false;              // si true: suena al terminar cada palabra; si false: suena por letra
    public float typeSoundPitchRandomness = 0.08f; // variación aleatoria en el pitch
    public float typeSoundDelay = 0.02f;          // retardo mínimo entre sonidos (útil si el text se escribe muy rápido)

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

        float lastSoundTime = -999f;

        for (int i = 0; i < fullText.Length; i++)
        {
            char currentChar = fullText[i];
            textUI.text += currentChar;

            // Sonido: por palabra o por letra
            if (typeSound != null && audioSource != null)
            {
                if (soundPerWord)
                {
                    // reproducir cuando se termina una palabra: al encontrarse un espacio
                    // y el carácter anterior no fuera espacio, o al llegar al último carácter si no es espacio
                    if (char.IsWhiteSpace(currentChar))
                    {
                        if (i > 0 && !char.IsWhiteSpace(fullText[i - 1]))
                        {
                            PlayTypeSound();
                            lastSoundTime = Time.time;
                        }
                    }
                    else if (i == fullText.Length - 1) // última letra del texto
                    {
                        // si el último carácter no es espacio, reproducimos para la última palabra
                        if (!char.IsWhiteSpace(currentChar))
                        {
                            PlayTypeSound();
                            lastSoundTime = Time.time;
                        }
                    }
                }
                else // por letra
                {
                    // evita reproducir en espacios y respeta el delay mínimo
                    if (!char.IsWhiteSpace(currentChar) && (Time.time - lastSoundTime >= typeSoundDelay))
                    {
                        PlayTypeSound();
                        lastSoundTime = Time.time;
                    }
                }
            }

            yield return new WaitForSeconds(charDelay);
        }

        OnTypingComplete();
    }

    void PlayTypeSound()
    {
        if (audioSource == null || typeSound == null) return;

        // variar pitch ligeramente para naturalidad
        audioSource.pitch = 1f + Random.Range(-typeSoundPitchRandomness, typeSoundPitchRandomness);
        audioSource.PlayOneShot(typeSound);
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