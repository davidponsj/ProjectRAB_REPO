using UnityEngine;

public class FinishLine : MonoBehaviour
{
    [SerializeField] private LevelTimer levelTimer; // Arrastraremos el LevelTimerManager aquí

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Solo si es el jugador
        {
            levelTimer.StopTimer(); // Detenemos el cronómetro y guardamos el mejor tiempo
            Debug.Log("Jugador llegó a la meta"); // Para depurar
        }
    }
}
