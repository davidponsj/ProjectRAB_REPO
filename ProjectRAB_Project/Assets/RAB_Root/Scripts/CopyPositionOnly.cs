using UnityEngine;

public class CopyPositionOnly : MonoBehaviour
{
    public Transform target;       // arrastra tu bola aquÅE    
    public Vector3 offset = Vector3.zero;
    public bool lateUpdateCopy = true;

    void LateUpdate()
    {
        if (target == null) return;
        transform.position = target.position + offset;
        // NOTA: no copiamos rotaciÛn
    }

    // Si prefieres Update en vez de LateUpdate, pon lateUpdateCopy = false y cambia a Update()
}