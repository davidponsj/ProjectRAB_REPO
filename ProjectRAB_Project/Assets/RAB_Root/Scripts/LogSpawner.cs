using UnityEngine;

public class LogSpawner : MonoBehaviour
{                                           // ESTE SCRIPT SE AÑADE A UN EMPTY OBJECT
    public GameObject Tronco;               // Prefab del tronco
    public float spawnInterval = 5f;        // Tiempo entre apariciones
    public Transform spawnPoint;            // Punto donde aparece el tronco (EL MISMO EMPTY OBJECT AL QUE LE AÑADES ESTE SCRIPT)

    private void Start()
    {
        InvokeRepeating(nameof(SpawnLog), 0f, spawnInterval);
    }

    void SpawnLog()
    {
        Instantiate(Tronco, spawnPoint.position, spawnPoint.rotation);
    }
}