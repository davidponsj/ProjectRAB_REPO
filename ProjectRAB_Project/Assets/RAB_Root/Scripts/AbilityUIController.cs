using UnityEngine;
using TMPro;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class AbilityUIController : MonoBehaviour
{
    [Header("Referencias (arrastra en el Inspector)")]
    public TextMeshProUGUI abilityText;     // Tu TMP text
    public RectTransform container;         // RectTransform que envuelve al texto (puede ser abilityText.rectTransform)
    public CanvasGroup canvasGroup;         // Opcional: para controlar alpha (se añadirá si falta)

    [Header("Animación")]
    public float slideDuration = 0.35f;
    public float visibleX = 0f;             // anchoredPosition.x cuando visible
    public float hiddenLeftX = -600f;       // fuera por la izquierda (ajusta según tu Canvas)
    public AnimationCurve ease = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Debug / pruebas")]
    public bool debugShowOnStart = false;   // si quieres forzar mostrar al iniciar (para depurar)
    public string debugAbilityName = "HighJump";
    public string debugKeyHint = "Espacio";
    public Color debugColor = Color.green;

    Coroutine currentCoroutine;
    Coroutine autoHideCoroutine;

    // Estado actual (para poder recolorear solo el nombre)
    string currentAbilityName = "";
    string currentKeyHint = "";
    Color currentNameColor = Color.green;

    void Reset()
    {
        // Intentamos auto-asignar referencias comunes para evitar errores
        if (abilityText == null)
            abilityText = GetComponentInChildren<TextMeshProUGUI>();
        if (container == null && abilityText != null)
            container = abilityText.rectTransform;
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    void Awake()
    {
        // Asegurar CanvasGroup
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
                canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // Intentar auto-asignar si hace falta
        if (abilityText == null)
            abilityText = GetComponentInChildren<TextMeshProUGUI>();

        if (container == null && abilityText != null)
            container = abilityText.rectTransform;

        // Si falta algo, avisamos en la consola para que lo arrastres en el Inspector
        if (abilityText == null || container == null)
        {
            Debug.LogWarning("[AbilityUIController] Faltan referencias: " +
                $"abilityText={(abilityText != null)} container={(container != null)} . Asigna en el Inspector.");
        }

        // Posición inicial fuera a la izquierda y oculto
        if (container != null)
        {
            Vector2 ap = container.anchoredPosition;
            ap.x = hiddenLeftX;
            container.anchoredPosition = ap;
        }

        if (abilityText != null) abilityText.text = "";
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
    }

    void Start()
    {
        if (debugShowOnStart)
        {
            Debug.Log("[AbilityUIController] debugShowOnStart activo: mostrando prueba.");
            ShowAbilityFormatted(debugAbilityName, debugKeyHint, debugColor, 0f);
        }
    }

    // --------------------------------------------------
    // API pública
    // --------------------------------------------------

    // Muestra la habilidad con formato: "Habilidad: <nombre> (tecla)"
    // nameColor colorea SOLO <nombre>
    // autoHideAfter: segundos reales (>0) para autoocultar, 0 = no auto-hide
    public void ShowAbilityFormatted(string abilityName, string keyHint, Color nameColor, float autoHideAfter = 0f)
    {
        if (abilityText == null || container == null)
        {
            Debug.LogWarning("[AbilityUIController] ShowAbilityFormatted llamado pero faltan referencias.");
            return;
        }

        Debug.Log($"[AbilityUIController] ShowAbilityFormatted(ability={abilityName}, keyHint={keyHint}, color={nameColor}, autoHideAfter={autoHideAfter})");

        currentAbilityName = abilityName;
        currentKeyHint = keyHint;
        currentNameColor = nameColor;
        UpdateTextWithColor();

        // cancelar auto-hide previo
        if (autoHideCoroutine != null) { StopCoroutine(autoHideCoroutine); autoHideCoroutine = null; }

        // iniciar animación entrada
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(DoSlideIn());

        // si pide auto-hide, lanzamos coroutine para ocultar luego
        if (autoHideAfter > 0f)
            autoHideCoroutine = StartCoroutine(AutoHideRoutine(autoHideAfter));
    }

    // Marca la habilidad como usada: colorea el nombre y oculta tras hideAfterSeconds
    public void MarkAbilityUsed(Color usedNameColor, float hideAfterSeconds = 1.5f)
    {
        if (abilityText == null)
        {
            Debug.LogWarning("[AbilityUIController] MarkAbilityUsed llamado pero abilityText es null.");
            return;
        }

        currentNameColor = usedNameColor;
        UpdateTextWithColor();

        // reiniciamos y lanzamos ocultado tras retardo real
        if (autoHideCoroutine != null) { StopCoroutine(autoHideCoroutine); autoHideCoroutine = null; }
        if (hideAfterSeconds > 0f) autoHideCoroutine = StartCoroutine(AutoHideRoutine(hideAfterSeconds));
    }

    // Oculta inmediatamente (slide out hacia la izquierda)
    public void HideAbility()
    {
        if (container == null || canvasGroup == null) return;
        if (autoHideCoroutine != null) { StopCoroutine(autoHideCoroutine); autoHideCoroutine = null; }
        if (currentCoroutine != null) StopCoroutine(currentCoroutine);
        currentCoroutine = StartCoroutine(DoSlideOut());
    }

    // Oculta después de segundos (tiempo real)
    public void HideAbilityDelayed(float seconds)
    {
        if (container == null) return;
        if (autoHideCoroutine != null) StopCoroutine(autoHideCoroutine);
        autoHideCoroutine = StartCoroutine(AutoHideRoutine(seconds));
    }

    // --------------------------------------------------
    // Coroutines internas
    // --------------------------------------------------
    IEnumerator AutoHideRoutine(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        autoHideCoroutine = null;
        HideAbility();
    }

    void UpdateTextWithColor()
    {
        if (abilityText == null) return;
        string hex = ColorUtility.ToHtmlStringRGB(currentNameColor);
        // Formato: Habilidad: <color=#HEX>Nombre</color> (tecla)
        abilityText.text = $"Habilidad: <color=#{hex}>{currentAbilityName}</color> ({currentKeyHint})";
    }

    IEnumerator DoSlideIn()
    {
        if (container == null || canvasGroup == null) yield break;

        float t = 0f;
        Vector2 startPos = container.anchoredPosition;
        Vector2 from = new Vector2(hiddenLeftX, startPos.y);
        Vector2 to = new Vector2(visibleX, startPos.y);

        // Asegurarnos de partir desde hiddenLeftX
        container.anchoredPosition = from;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        while (t < slideDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / slideDuration);
            float e = ease.Evaluate(p);
            container.anchoredPosition = Vector2.Lerp(from, to, e);
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, e);
            yield return null;
        }

        container.anchoredPosition = to;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        currentCoroutine = null;
    }

    IEnumerator DoSlideOut()
    {
        if (container == null || canvasGroup == null) yield break;

        float t = 0f;
        Vector2 startPos = container.anchoredPosition;
        Vector2 from = startPos;
        Vector2 to = new Vector2(hiddenLeftX, startPos.y); // sale hacia la izquierda

        canvasGroup.blocksRaycasts = false;

        while (t < slideDuration)
        {
            t += Time.unscaledDeltaTime;
            float p = Mathf.Clamp01(t / slideDuration);
            float e = ease.Evaluate(p);
            container.anchoredPosition = Vector2.Lerp(from, to, e);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, e);
            yield return null;
        }

        container.anchoredPosition = to;
        canvasGroup.alpha = 0f;
        abilityText.text = "";
        currentCoroutine = null;
    }

    // Para facilitar ajuste desde el editor: al cambiar valores en el inspector, actualizamos la posición si el container existe
#if UNITY_EDITOR
    void OnValidate()
    {
        if (!Application.isPlaying && container != null)
        {
            Vector2 ap = container.anchoredPosition;
            ap.x = hiddenLeftX;
            container.anchoredPosition = ap;
        }
    }
#endif
}
