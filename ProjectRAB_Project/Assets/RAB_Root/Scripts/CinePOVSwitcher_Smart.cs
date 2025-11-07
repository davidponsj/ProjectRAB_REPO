using System.Reflection;
using UnityEngine;

public class CinePOVSwitcher_Smart : MonoBehaviour
{
    [Header("Cameras (GameObjects)")]
    public GameObject normalCamObject;    // CM_Normal (GameObject)
    public GameObject povCamObject;       // CM_POV (GameObject)

    [Header("Player")]
    public Transform player;              // player transform (o proxy si usas uno)
    public LayerMask obstacleMask;        // capas que cuentan como plataforma/obstáculo
    public LayerMask groundMask;          // capas que cuentan como "suelo" (para comprobar collider bajo los pies)

    [Header("Detección")]
    public float detectDistance = 6f;     // hasta dónde buscamos la plataforma delante
    public float detectRadius = 0.5f;     // spherecast radius
    public float detectHeight = 1.0f;     // altura del origen de la spherecast respecto al jugador

    [Header("Condiciones para considerarlo 'otra plataforma'")]
    public float minGapDistance = 1.2f;   // distancia horizontal mínima para considerar salto a otra plataforma
    public float minHeightDiff = 0.35f;   // diferencia de altura mínima para considerarlo distinto (m)
    public bool requireJumpPress = true;  // true = sólo activar si el jugador pulsa salto
    public bool autoPreview = false;      // true = activar POV si se aproxima incluso sin pulsar salto
    public float preViewTime = 0.25f;     // tiempo que se mantiene la POV tras detectar

    [Header("Debug")]
    public bool drawDebug = true;

    // reflection stuff for Priority (opcional)
    PropertyInfo normalPriorityProp;
    PropertyInfo povPriorityProp;
    Component normalCamComponentWithProp;
    Component povCamComponentWithProp;

    float povTimer = 0f;

    void Start()
    {
        // cachear componente con propiedad "Priority" si existe
        if (normalCamObject != null)
            normalCamComponentWithProp = FindComponentWithProperty(normalCamObject, "Priority");
        if (povCamObject != null)
            povCamComponentWithProp = FindComponentWithProperty(povCamObject, "Priority");

        if (normalCamComponentWithProp != null)
            normalPriorityProp = normalCamComponentWithProp.GetType().GetProperty("Priority");
        if (povCamComponentWithProp != null)
            povPriorityProp = povCamComponentWithProp.GetType().GetProperty("Priority");
    }

    void Update()
    {
        if (player == null) return;

        // input salto
        bool jumpPressed = requireJumpPress ? Input.GetButton("Jump") : false;

        // calculamos origen y direccion
        Vector3 origin = player.position + Vector3.up * detectHeight;
        Vector3 dir = player.forward;

        // spherecast hacia delante
        bool hitPlatform = Physics.SphereCast(origin, detectRadius, dir, out RaycastHit hit, detectDistance, obstacleMask);

        bool shouldActivatePOV = false;

        if (hitPlatform)
        {
            // collider detectado
            Collider hitCol = hit.collider;

            // comprobar collider bajo los pies del jugador (raycast hacia abajo)
            Collider groundUnderPlayer = null;
            if (Physics.Raycast(player.position + Vector3.up * 0.1f, Vector3.down, out RaycastHit groundHit, 2f, groundMask))
            {
                groundUnderPlayer = groundHit.collider;
            }

            // condición 1: es distinto collider al que estamos apoyados (evita detectar el mismo suelo)
            bool differentCollider = (groundUnderPlayer == null) || (groundUnderPlayer != hitCol);

            // distancia horizontal al punto de impacto (proyección XZ)
            Vector3 playerPosXZ = new Vector3(player.position.x, 0f, player.position.z);
            Vector3 hitPointXZ = new Vector3(hit.point.x, 0f, hit.point.z);
            float horizDist = Vector3.Distance(playerPosXZ, hitPointXZ);

            // diferencia de altura (hit.point.y - player.position.y)
            float heightDiff = hit.point.y - player.position.y;

            // condición extra: gap o diferencia de altura suficiente
            bool isSignificantGap = horizDist >= minGapDistance || Mathf.Abs(heightDiff) >= minHeightDiff;

            if (differentCollider && isSignificantGap)
            {
                // si requiere pulsar salto, activamos solo con jumpPressed,
                // si no, activamos si se aproxima (autoPreview) o si se ha pulsado jump
                if (requireJumpPress)
                {
                    if (jumpPressed)
                    {
                        shouldActivatePOV = true;
                        povTimer = preViewTime;
                    }
                    else if (autoPreview)
                    {
                        // previsualizar aunque no haya pulsado todavía
                        shouldActivatePOV = true;
                        povTimer = preViewTime;
                    }
                }
                else
                {
                    // no requiere pulsar salto: activamos si se aproxima o por timer
                    shouldActivatePOV = true;
                    povTimer = preViewTime;
                }

                if (drawDebug)
                {
                    Debug.Log($"Hit platform: horizDist={horizDist:F2}, heightDiff={heightDiff:F2}, differentCollider={differentCollider}");
                }
            }
        }

        // mantener POV si povTimer > 0 (breve previsualizado)
        if (povTimer > 0f)
        {
            povTimer -= Time.deltaTime;
            shouldActivatePOV = true;
        }

        // aplicar cambio (Priority via reflection si existe, si no usar SetActive fallback)
        if (shouldActivatePOV)
            SetPOVActive();
        else
            SetNormalActive();

        if (drawDebug)
        {
            Debug.DrawRay(origin, dir * detectDistance, hitPlatform ? Color.red : Color.green);
            // dibujar esfera de detección
            GizmoDrawSphere(origin + dir * detectDistance, detectRadius, drawDebug && hitPlatform);
        }
    }

    // reflection + fallback
    void SetPOVActive()
    {
        if (povPriorityProp != null && normalPriorityProp != null)
        {
            povPriorityProp.SetValue(povCamComponentWithProp, 20, null);
            normalPriorityProp.SetValue(normalCamComponentWithProp, 10, null);
            return;
        }
        if (povCamObject != null) povCamObject.SetActive(true);
        if (normalCamObject != null) normalCamObject.SetActive(false);
    }

    void SetNormalActive()
    {
        if (povPriorityProp != null && normalPriorityProp != null)
        {
            povPriorityProp.SetValue(povCamComponentWithProp, 10, null);
            normalPriorityProp.SetValue(normalCamComponentWithProp, 20, null);
            return;
        }
        if (povCamObject != null) povCamObject.SetActive(false);
        if (normalCamObject != null) normalCamObject.SetActive(true);
    }

    Component FindComponentWithProperty(GameObject go, string propName)
    {
        var comps = go.GetComponents<Component>();
        foreach (var c in comps)
        {
            if (c == null) continue;
            var pi = c.GetType().GetProperty(propName);
            if (pi != null) return c;
        }
        return null;
    }

    void GizmoDrawSphere(Vector3 pos, float r, bool hit)
    {
        // no hacemos Gizmos reales fuera de OnDrawGizmos, pero dejamos un Debug.DrawRay para visibilidad.
        // Si quieres Gizmos en Scene view, implemento OnDrawGizmosSelected.
    }
}
