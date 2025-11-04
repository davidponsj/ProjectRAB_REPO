using UnityEngine;
using System.Collections;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public enum AbilityType { None, Dash, HighJump, SlowTime }

[RequireComponent(typeof(UnityEngine.Rigidbody))]
public class PlayerBallController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveForce = 15f;
    [SerializeField] private float maxSpeed = 12f;

    [Header("Salto")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float highJumpMultiplier = 1.8f;
    [SerializeField] private float groundCheckDistance = 0.6f;
    [SerializeField] private LayerMask groundMask;

    [Header("Dash (1 uso)")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.15f;
    [SerializeField] private float dashCooldown = 0.1f;
    [SerializeField] private float doubleTapWindow = 0.25f;

    [Header("Slow Time")]
    [SerializeField] private float slowTimeScale = 0.35f;
    [SerializeField] private float slowTimeDuration = 2.0f;
    [SerializeField] private float slowTimeCooldown = 0.5f;

    [Header("VFX")]
    [SerializeField] private ParticleSystem dashParticles;

    [Header("UI")]
    [SerializeField] private AbilityUIController ui;

    private UnityEngine.Rigidbody rb;
    private UnityEngine.Vector3 lastMoveDir = UnityEngine.Vector3.forward;
    private float lastSpaceDownTime = -999f;
    private bool isDashing = false;
    private bool dashOnCooldown = false;
    private bool slowOnCooldown = false;

    private float originalFixedDeltaTime = 0.02f;

    public AbilityType currentAbility { get; private set; } = AbilityType.None;

    void Awake()
    {
        rb = GetComponent<UnityEngine.Rigidbody>();
        originalFixedDeltaTime = Time.fixedDeltaTime;
        if (dashParticles != null)
            dashParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        UpdateUI();
    }

    void Update()
    {
        HandleJumpAndDashInput();
        HandleOtherAbilityInput();
    }

    void FixedUpdate()
    {
        HandleMovement();
        ClampHorizontalSpeed();
    }

    void HandleMovement()
    {
        float h = 0f, v = 0f;

#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.aKey.isPressed || kb.leftArrowKey.isPressed) h -= 1f;
            if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) h += 1f;
            if (kb.sKey.isPressed || kb.downArrowKey.isPressed) v -= 1f;
            if (kb.wKey.isPressed || kb.upArrowKey.isPressed) v += 1f;
        }
        else
        {
            h = Input.GetAxisRaw("Horizontal");
            v = Input.GetAxisRaw("Vertical");
        }
#else
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
#endif

        UnityEngine.Vector3 input = new UnityEngine.Vector3(h, 0f, v).normalized;

        if (input.sqrMagnitude > 0.001f)
        {
            lastMoveDir = input;
            rb.AddForce(input * moveForce, ForceMode.Acceleration);
        }
    }

    void HandleJumpAndDashInput()
    {
        bool spaceDown = Input.GetKeyDown(KeyCode.Space);
        if (!spaceDown) return;

        float t = Time.time;
        bool doubleTap = (t - lastSpaceDownTime) <= doubleTapWindow;
        lastSpaceDownTime = t;

        Debug.Log($"[Input] Space pressed. currentAbility={currentAbility}");

        // Primero comprobamos HighJump
        bool grounded = IsGrounded(); // IsGrounded dibuja un raycast en escena
        Debug.Log($"[Input] IsGrounded = {grounded}");

        if (currentAbility == AbilityType.HighJump && grounded)
        {
            // Aplicamos HighJump con AddForce (Impulse) para que se note.
            float highJumpForce = jumpForce * highJumpMultiplier; // ejemplo: jumpForce=6, multiplier=2 => 12
            Debug.Log($"[HighJump] Activado. highJumpForce={highJumpForce}, mass={rb.mass}");
            rb.AddForce(Vector3.up * highJumpForce, ForceMode.Impulse);

            // Consumir la habilidad y actualizar UI inmediatamente
            currentAbility = AbilityType.None;
            UpdateUI();
            Debug.Log("[HighJump] Habilidad consumida.");
            return;
        }

        // Dash por doble tap (no tocamos si no tienes Dash)
        if (doubleTap && currentAbility == AbilityType.Dash && !isDashing && !dashOnCooldown)
        {
            StartCoroutine(DashCoroutine());
            return;
        }

        // Si llegamos aquí: No tienes HighJump (o no estabas en suelo)
        if (grounded)
        {
            Debug.Log("[Jump] Salto normal aplicado.");
            rb.AddForce(Vector3.up * (jumpForce * rb.mass), ForceMode.Impulse);
        }
        else
        {
            Debug.Log("[Jump] No grounded -> no salto.");
        }
    }

    void HandleOtherAbilityInput()
    {
        bool ePressed = false;

#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb != null) ePressed = kb.eKey.wasPressedThisFrame;
        else ePressed = Input.GetKeyDown(KeyCode.E);
#else
        ePressed = Input.GetKeyDown(KeyCode.E);
#endif

        if (ePressed && currentAbility == AbilityType.SlowTime && !slowOnCooldown)
        {
            StartCoroutine(SlowTimeCoroutine());
            currentAbility = AbilityType.None;
            UpdateUI();
        }
    }

    bool IsGrounded()
    {
        UnityEngine.Vector3 origin = transform.position + UnityEngine.Vector3.up * 0.1f;
        float checkDist = groundCheckDistance + 0.1f;
        bool hit = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask);

        Debug.DrawRay(transform.position, Vector3.down * groundCheckDistance, Color.green);
        return hit;
    }

    void ClampHorizontalSpeed()
    {
        UnityEngine.Vector3 vel = rb.linearVelocity;
        UnityEngine.Vector3 horizontal = new UnityEngine.Vector3(vel.x, 0f, vel.z);
        if (horizontal.magnitude > maxSpeed && !isDashing)
        {
            UnityEngine.Vector3 clamped = horizontal.normalized * maxSpeed;
            rb.linearVelocity = new UnityEngine.Vector3(clamped.x, vel.y, clamped.z);
        }
    }

    IEnumerator DashCoroutine()
    {
        if (isDashing) yield break;
        isDashing = true;
        dashOnCooldown = true;

        UnityEngine.Vector3 dashDir = lastMoveDir.sqrMagnitude > 0.001f ? lastMoveDir.normalized : UnityEngine.Vector3.forward;

        if (dashParticles != null)
        {
            var main = dashParticles.main;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            dashParticles.transform.localRotation = Quaternion.LookRotation(-dashDir, UnityEngine.Vector3.up);
            dashParticles.Play();
        }

        UnityEngine.Vector3 currentVel = rb.linearVelocity;
        UnityEngine.Vector3 newVel = new UnityEngine.Vector3(dashDir.x * dashForce, currentVel.y, dashDir.z * dashForce);
        rb.linearVelocity = newVel;

        currentAbility = AbilityType.None;
        UpdateUI();

        yield return new WaitForSeconds(dashDuration);

        if (dashParticles != null)
            dashParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        dashOnCooldown = false;
    }

    IEnumerator SlowTimeCoroutine()
    {
        if (slowOnCooldown) yield break;
        slowOnCooldown = true;

        float previousTimeScale = Time.timeScale;
        float previousFixed = Time.fixedDeltaTime;

        Time.timeScale = slowTimeScale;
        Time.fixedDeltaTime = originalFixedDeltaTime * slowTimeScale;

        yield return new WaitForSecondsRealtime(slowTimeDuration);

        Time.timeScale = previousTimeScale;
        Time.fixedDeltaTime = previousFixed;

        yield return new WaitForSeconds(slowTimeCooldown);
        slowOnCooldown = false;
    }

    public void GiveAbility(AbilityType ability)
    {
        currentAbility = ability;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (ui != null)
        {
            string t = currentAbility == AbilityType.None ? "Sin habilidad" : currentAbility.ToString();
            if (currentAbility == AbilityType.Dash) t += " (doble espacio)";
            else if (currentAbility == AbilityType.HighJump) t += " (usa Espacio en suelo)";
            else if (currentAbility == AbilityType.SlowTime) t += " (pulsa E)";
            ui.SetAbilityText(t);
        }
    }
}


