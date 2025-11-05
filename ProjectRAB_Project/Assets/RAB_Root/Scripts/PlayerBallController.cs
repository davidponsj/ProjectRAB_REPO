using UnityEngine;
using System.Collections;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem; // para el nuevo sistema de entrada
#endif

public enum AbilityType { None, Dash, HighJump, SlowTime }

[RequireComponent(typeof(Rigidbody))]
public class PlayerBallController : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveForce = 15f;
    public float maxSpeed = 12f;

    [Header("Salto")]
    public float jumpForce = 6f;
    public float highJumpMultiplier = 2f; // salto extra
    public float groundCheckDistance = 0.6f;
    public LayerMask groundMask;

    [Header("Dash (1 uso)")]
    public float dashForce = 15f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 0.1f;
    public float doubleTapWindow = 0.25f;

    [Header("Slow Time")]
    public float slowTimeScale = 0.4f;
    public float slowTimeDuration = 1.5f;
    public float slowTimeCooldown = 1f;

    [Header("VFX")]
    public ParticleSystem dashParticles;

    [Header("UI")]
    public AbilityUIController ui;

    private Rigidbody rb;
    private Vector3 lastMoveDir = Vector3.forward;
    private float lastSpaceDownTime = -999f;
    private bool isDashing = false;
    private bool dashOnCooldown = false;
    private bool slowOnCooldown = false;
    private bool usedHighJump = false;
    private float originalFixedDeltaTime = 0.02f;

    public AbilityType currentAbility { get; private set; } = AbilityType.None;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalFixedDeltaTime = Time.fixedDeltaTime;

        if (dashParticles != null)
            dashParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (ui != null)
            ui.HideAbility(); // empieza oculto
    }

    void Update()
    {
        HandleJumpAndDashInput();
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
#else
        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
#endif

        Vector3 input = new Vector3(h, 0f, v).normalized;

        if (input.sqrMagnitude > 0.001f)
        {
            lastMoveDir = input;
            rb.AddForce(input * moveForce, ForceMode.Acceleration);
        }
    }

    void HandleJumpAndDashInput()
    {
        bool spaceDown = false;

#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb != null) spaceDown = kb.spaceKey.wasPressedThisFrame;
#else
        spaceDown = Input.GetKeyDown(KeyCode.Space);
#endif

        if (spaceDown)
        {
            float t = Time.time;
            bool doubleTap = (t - lastSpaceDownTime) <= doubleTapWindow;
            lastSpaceDownTime = t;

            // Dash
            if (doubleTap && currentAbility == AbilityType.Dash && !isDashing && !dashOnCooldown)
            {
                StartCoroutine(DashCoroutine());
                return;
            }

            // Salto normal o alto
            if (IsGrounded())
            {
                float appliedForce = jumpForce;

                if (currentAbility == AbilityType.HighJump && !usedHighJump)
                {
                    appliedForce *= highJumpMultiplier;
                    usedHighJump = true;

                    // Cambiar color del texto a rojo y ocultar tras 2s
                    if (ui != null)
                        ui.MarkAbilityUsed(Color.red, 2f);

                    currentAbility = AbilityType.None;
                }

                rb.AddForce(Vector3.up * (appliedForce * rb.mass), ForceMode.Impulse);
            }
        }

#if ENABLE_INPUT_SYSTEM
        if (kb != null && kb.leftShiftKey.wasPressedThisFrame && currentAbility == AbilityType.SlowTime && !slowOnCooldown)
#else
        if (Input.GetKeyDown(KeyCode.LeftShift) && currentAbility == AbilityType.SlowTime && !slowOnCooldown)
#endif
        {
            StartCoroutine(SlowTimeCoroutine());
        }
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundMask, QueryTriggerInteraction.Ignore);
    }

    void ClampHorizontalSpeed()
    {
        Vector3 vel = rb.linearVelocity;
        Vector3 horizontal = new Vector3(vel.x, 0f, vel.z);
        if (horizontal.magnitude > maxSpeed && !isDashing)
        {
            Vector3 clamped = horizontal.normalized * maxSpeed;
            rb.linearVelocity = new Vector3(clamped.x, vel.y, clamped.z);
        }
    }

    IEnumerator DashCoroutine()
    {
        isDashing = true;
        dashOnCooldown = true;

        Vector3 dashDir = lastMoveDir.sqrMagnitude > 0.001f ? lastMoveDir.normalized : Vector3.forward;

        // Partículas
        if (dashParticles != null)
        {
            var main = dashParticles.main;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            dashParticles.transform.forward = -dashDir;
            dashParticles.Play();
        }

        rb.AddForce(dashDir * dashForce, ForceMode.VelocityChange);

        yield return new WaitForSeconds(dashDuration);

        if (dashParticles != null)
            dashParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        isDashing = false;

        if (ui != null)
            ui.MarkAbilityUsed(Color.red, 1.5f);

        currentAbility = AbilityType.None;

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

        if (ui != null)
            ui.MarkAbilityUsed(Color.red, 2f);

        yield return new WaitForSecondsRealtime(slowTimeDuration);

        Time.timeScale = previousTimeScale;
        Time.fixedDeltaTime = previousFixed;

        currentAbility = AbilityType.None;

        yield return new WaitForSecondsRealtime(slowTimeCooldown);
        slowOnCooldown = false;
    }

    public void GiveAbility(AbilityType ability)
    {
        currentAbility = ability;
        usedHighJump = false;

        string keyHint = "";

        switch (ability)
        {
            case AbilityType.Dash:
                keyHint = "doble Espacio";
                break;
            case AbilityType.HighJump:
                keyHint = "Espacio";
                break;
            case AbilityType.SlowTime:
                keyHint = "Shift Izquierdo";
                break;
        }

        if (ui != null)
            ui.ShowAbilityFormatted(ability.ToString(), keyHint, Color.green);
    }
}

