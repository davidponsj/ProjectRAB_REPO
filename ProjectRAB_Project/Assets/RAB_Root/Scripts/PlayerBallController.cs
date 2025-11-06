using UnityEngine;
using System.Collections;

public enum AbilityType { None, Dash, HighJump, SlowTime }

[RequireComponent(typeof(Rigidbody))]
public class PlayerBallController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float moveSpeed = 6f;       // velocidad horizontal objetivo (m/s)
    [SerializeField] private float moveForce = 18f;     // (se usa solo fuera de slow)
    [SerializeField] private float maxSpeed = 12f;

    [Header("Salto")]
    [SerializeField] private float jumpForce = 6f;      // velocidad vertical objetivo
    [SerializeField] private float highJumpMultiplier = 1.8f;

    [Header("Ground Check")]
    [SerializeField] private float groundCheckRadius = 0.45f;
    [SerializeField] private float groundCheckOffset = 0.1f;
    [SerializeField] private LayerMask groundMask;

    [Header("Dash (1 uso)")]
    [SerializeField] private float dashForce = 15f;
    [SerializeField] private float dashDuration = 0.12f;
    [SerializeField] private float dashCooldown = 0.15f;
    [SerializeField] private float doubleTapWindow = 0.25f;

    [Header("Slow Time")]
    [SerializeField] private float slowTimeScale = 0.35f;   // cuánto se ralentiza el mundo
    [SerializeField] private float slowTimeDuration = 2.0f; // segundos reales
    [SerializeField] private float slowTimeCooldown = 0.5f; // segundos reales
    [SerializeField] private KeyCode slowActivateKey = KeyCode.LeftShift;

    [Header("VFX")]
    [SerializeField] private ParticleSystem dashParticles;

    [Header("UI")]
    [SerializeField] private AbilityUIController ui;

    [Header("Debug")]
    public bool debugLogs = false;
    [Tooltip("Máximo factor de compensación (1/timeScale) permitido para evitar impulsos extremos")]
    public float maxComp = 6f;

    // internals
    private Rigidbody rb;
    private Vector3 lastMoveDir = Vector3.forward;
    private float lastSpaceDownTime = -999f;
    private bool isDashing = false;
    private bool dashOnCooldown = false;
    private bool slowOnCooldown = false;

    private float originalFixedDeltaTime;
    private float originalTimeScale = 1f;

    // estado slow
    private bool slowTimeActive = false;

    public AbilityType currentAbility { get; private set; } = AbilityType.None;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.interpolation = RigidbodyInterpolation.Interpolate;

        originalFixedDeltaTime = Time.fixedDeltaTime;
        originalTimeScale = Time.timeScale;

        if (dashParticles != null)
            dashParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        if (ui != null) ui.HideAbility();
    }

    void Update()
    {
        HandleJumpAndDashInput();
        HandleOtherAbilityInput();
    }

    void FixedUpdate()
    {
        // Movimiento continuo: fuera de slow usamos AddForce; dentro slow ajustamos linearVelocity directamente
        if (slowTimeActive)
            HandleMovement_SlowCompensated();
        else
            HandleMovement_Normal();

        // Clamp horizontal
        ClampHorizontalSpeed();

        // Gravity compensation during slow so falling feels normal
        if (slowTimeActive && rb != null)
        {
            float ts = Mathf.Max(Time.timeScale, 0.0001f);
            float comp = Mathf.Clamp(1f / ts, 1f, maxComp);
            Vector3 extraAccel = (comp - 1f) * Physics.gravity;
            rb.AddForce(extraAccel * rb.mass, ForceMode.Acceleration);

            if (debugLogs)
                Debug.Log($"[Player] Gravity comp applied comp={comp} extraAccel={extraAccel}");
        }
    }

    // ---------- Movement modes ----------
    void HandleMovement_Normal()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0f, v).normalized;
        if (input.sqrMagnitude > 0.001f)
        {
            lastMoveDir = input;
            rb.AddForce(input * moveForce, ForceMode.Acceleration);
        }
    }

    // During slow: set horizontal velocity directly (compensated) for instant response
    void HandleMovement_SlowCompensated()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 input = new Vector3(h, 0f, v).normalized;

        float ts = Mathf.Max(Time.timeScale, 0.0001f);
        float comp = Mathf.Clamp(1f / ts, 1f, maxComp);

        Vector3 lv = rb.linearVelocity;
        Vector3 desiredHorizVel = input * moveSpeed * comp; // immediate horizontal speed in real time
        // Smooth approach: keep current y, set xz to desired
        lv.x = desiredHorizVel.x;
        lv.z = desiredHorizVel.z;
        rb.linearVelocity = lv;

        if (input.sqrMagnitude > 0.001f)
            lastMoveDir = input;

        if (debugLogs)
            Debug.Log($"[Player] SlowMove: set lv.xz = {desiredHorizVel.x:F2},{desiredHorizVel.z:F2} comp={comp}");
    }

    // ---------- Input: Jump & Dash ----------
    void HandleJumpAndDashInput()
    {
        bool spaceDown = Input.GetKeyDown(KeyCode.Space);
        if (!spaceDown) return;

        float t = Time.time;
        bool doubleTap = (t - lastSpaceDownTime) <= doubleTapWindow;
        lastSpaceDownTime = t;

        bool grounded = IsGrounded();

        // Dash (double tap)
        if (doubleTap && currentAbility == AbilityType.Dash && !isDashing && !dashOnCooldown)
        {
            StartCoroutine(DashCoroutine());
            return;
        }

        // HighJump
        if (currentAbility == AbilityType.HighJump && grounded)
        {
            float targetJump = jumpForce * highJumpMultiplier;
            ApplyJumpImmediate(targetJump);
            currentAbility = AbilityType.None;
            if (ui != null) ui.MarkAbilityUsed(Color.red, 1.5f);
            return;
        }

        // Normal jump
        if (grounded)
        {
            ApplyJumpImmediate(jumpForce);
        }
    }

    // Apply jump by setting vertical linearVelocity (immediate) with compensation during slow
    void ApplyJumpImmediate(float baseJump)
    {
        float comp = slowTimeActive ? Mathf.Clamp(1f / Mathf.Max(Time.timeScale, 0.0001f), 1f, maxComp) : 1f;
        Vector3 lv = rb.linearVelocity;
        lv.y = baseJump * comp;
        rb.linearVelocity = lv;

        if (debugLogs)
            Debug.Log($"[Player] JumpImmediate base={baseJump} comp={comp} newY={lv.y}");
    }

    // ---------- Dash ----------
    IEnumerator DashCoroutine()
    {
        if (isDashing) yield break;
        isDashing = true;
        dashOnCooldown = true;

        Vector3 dashDir = lastMoveDir.sqrMagnitude > 0.001f ? lastMoveDir.normalized : Vector3.forward;

        if (dashParticles != null)
        {
            var main = dashParticles.main;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            dashParticles.transform.localRotation = Quaternion.LookRotation(-dashDir, Vector3.up);
            dashParticles.Play();
        }

        float comp = slowTimeActive ? Mathf.Clamp(1f / Mathf.Max(Time.timeScale, 0.0001f), 1f, maxComp) : 1f;
        float appliedDash = dashForce * comp;

        rb.AddForce(dashDir * appliedDash, ForceMode.VelocityChange);

        if (ui != null) ui.MarkAbilityUsed(Color.red, 1.5f);
        currentAbility = AbilityType.None;

        yield return new WaitForSeconds(dashDuration);

        if (dashParticles != null) dashParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        isDashing = false;
        yield return new WaitForSeconds(dashCooldown);
        dashOnCooldown = false;
    }

    // ---------- SlowTime Coroutine ----------
    IEnumerator SlowTimeCoroutine()
    {
        if (slowOnCooldown) yield break;
        slowOnCooldown = true;
        slowTimeActive = true;

        float target = Mathf.Clamp(slowTimeScale, 0.01f, 1f);
        float comp = Mathf.Clamp(1f / Mathf.Max(target, 0.0001f), 1f, maxComp);

        // store before velocities for debugging if needed
        Vector3 beforeVel = rb.linearVelocity;

        // apply global slow
        Time.timeScale = target;
        Time.fixedDeltaTime = originalFixedDeltaTime * Time.timeScale;

        // immediately scale horizontal velocity so player feels same speed
        Vector3 v = rb.linearVelocity;
        v.x *= comp;
        v.z *= comp;
        rb.linearVelocity = v;

        if (debugLogs) Debug.Log($"[Player] Slow start comp={comp} beforeVel={beforeVel} nowVel={v}");

        // duration (real time)
        yield return new WaitForSecondsRealtime(slowTimeDuration);

        // restore horizontal velocity (divide by comp)
        Vector3 vNow = rb.linearVelocity;
        float safeComp = Mathf.Clamp(comp, 1f, 12f);
        vNow.x /= safeComp;
        vNow.z /= safeComp;
        rb.linearVelocity = vNow;

        // restore time
        Time.timeScale = originalTimeScale;
        Time.fixedDeltaTime = originalFixedDeltaTime;

        slowTimeActive = false;

        if (debugLogs) Debug.Log($"[Player] Slow end restored velocity -> {vNow}");

        // cooldown (real)
        yield return new WaitForSecondsRealtime(slowTimeCooldown);
        slowOnCooldown = false;
    }

    void HandleOtherAbilityInput()
    {
        if (Input.GetKeyDown(slowActivateKey) && currentAbility == AbilityType.SlowTime && !slowOnCooldown)
        {
            currentAbility = AbilityType.None;
            if (ui != null) ui.MarkAbilityUsed(Color.red, 1.5f);
            StartCoroutine(SlowTimeCoroutine());
        }
    }

    // ---------- Helpers ----------
    bool IsGrounded()
    {
        Vector3 origin = transform.position + Vector3.down * groundCheckOffset;
        bool grounded = Physics.CheckSphere(origin, groundCheckRadius, groundMask, QueryTriggerInteraction.Ignore);
#if UNITY_EDITOR
        Debug.DrawRay(transform.position, Vector3.down * (groundCheckOffset + groundCheckRadius), grounded ? Color.green : Color.red);
#endif
        return grounded;
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

    void OnTriggerEnter(Collider other)
    {
        var pickup = other.GetComponent<AbilityPickup>();
        if (pickup != null)
        {
            GiveAbility(pickup.ability);
            Destroy(pickup.gameObject);
        }
    }

    public void GiveAbility(AbilityType ability)
    {
        currentAbility = ability;

        if (ui != null)
        {
            if (currentAbility == AbilityType.None)
            {
                ui.HideAbility();
                return;
            }

            string keyHint = "";
            if (ability == AbilityType.Dash) keyHint = "doble Espacio";
            else if (ability == AbilityType.HighJump) keyHint = "Espacio";
            else if (ability == AbilityType.SlowTime) keyHint = "Shift Izquierdo";

            ui.ShowAbilityFormatted(ability.ToString(), keyHint, Color.green);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 origin = transform.position + Vector3.down * groundCheckOffset;
        Gizmos.DrawWireSphere(origin, groundCheckRadius);
    }
}