using UnityEngine;

public class SpearTrapUltraSimple : MonoBehaviour
{
    public Transform spear;             // Lanza que se moverá
    public float moveDistance = 2f;     // Distancia del movimiento
    public float speed = 2f;            // Velocidad del movimiento
    public float startDelay = 0f;       // Retraso inicial

    public enum DirectionAxis { Forward, Backward, Right, Left, Up, Down }
    public DirectionAxis moveDirection = DirectionAxis.Forward; // Eje de movimiento

    private Vector3 startPos;

    void Start()
    {
        if (spear == null)
            spear = transform.GetChild(0);

        startPos = spear.localPosition;
    }

    void Update()
    {
        if (Time.time < startDelay) return;

        float offset = Mathf.PingPong((Time.time - startDelay) * speed, moveDistance);
        Vector3 dir = GetDirectionVector();

        spear.localPosition = startPos + dir * offset;
    }

    private Vector3 GetDirectionVector()
    {
        switch (moveDirection)
        {
            case DirectionAxis.Forward: return transform.forward;
            case DirectionAxis.Backward: return -transform.forward;
            case DirectionAxis.Right: return transform.right;
            case DirectionAxis.Left: return -transform.right;
            case DirectionAxis.Up: return transform.up;
            case DirectionAxis.Down: return -transform.up;
            default: return transform.forward;
        }
    }
}
