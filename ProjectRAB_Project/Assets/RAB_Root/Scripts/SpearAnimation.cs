using UnityEngine;

public class SpearTrapUltraSimple : MonoBehaviour
{
    public Transform spear;        
    public float moveDistance = 2f; 
    public float speed = 2f;       
    public float startDelay = 0f;  

    private Vector3 startPos;

    void Start()
    {
        if (spear == null)
            spear = transform.GetChild(0);
        startPos = spear.localPosition;
    }

    void Update()
    {
        if (Time.time < startDelay) return; // espera inicial

        // Movimiento oscilante simple (sale y entra)
        float offset = Mathf.PingPong((Time.time - startDelay) * speed, moveDistance);
        spear.localPosition = startPos + spear.forward * offset;
    }
}
