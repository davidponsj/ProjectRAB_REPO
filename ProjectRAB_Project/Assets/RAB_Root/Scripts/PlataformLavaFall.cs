using UnityEngine;

public class PlataformLavaFall : MonoBehaviour
{
    [Header("Tiempo antes de caer")]
        public float fallTime = 1f;
    [Header("Velocidad caida")]
    public float fallSpeed = 5f;

    private Rigidbody rb;
    private bool siJugadorpisa = false;
    private bool siseCae = false;
    private float temporizador = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    //Cuando el jugador toca la plataforma empieza con contanador invisible    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
