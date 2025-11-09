using UnityEngine;

public class CopyPositionOnly : MonoBehaviour
{
    public Transform target;       // Arrastra tu bola aquí
    public Vector3 offset = Vector3.zero;
    public bool lateUpdateCopy = true;

    void LateUpdate()
    {
        if (target == null) return;
        transform.position = target.position + offset;
        // NOTA: no copiamos rotación
    }

    // Si prefieres Update en vez de LateUpdate, pon lateUpdateCopy = false y cambia a Update()
}