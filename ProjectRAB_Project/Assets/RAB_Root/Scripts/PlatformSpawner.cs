using UnityEngine;
using System.Collections.Generic;

public class MovingPlatformSpawner : MonoBehaviour
{
    [Header("Plataforma")]
    public GameObject platformPrefab; // Prefab con todos los scripts que quieras
    public Transform startPoint;      // Donde aparecerán
    public Transform endPoint;        // Donde desaparecerán

    [Header("Generación")]
    public float spawnInterval = 2f;  // Cada cuánto se genera
    public int maxPlatforms = 10;     // Máximo de plataformas activas

    private List<GameObject> activePlatforms = new List<GameObject>();

    void Start()
    {
        InvokeRepeating(nameof(SpawnPlatform), 0f, spawnInterval);
    }

    void SpawnPlatform()
    {
        // Solo genera si no supera el límite
        if (activePlatforms.Count >= maxPlatforms) return;

        // Instancia la plataforma
        GameObject newPlatform = Instantiate(platformPrefab, startPoint.position, platformPrefab.transform.rotation);

        // Asigna el EndPoint al script de la plataforma
        MovingPlatform mp = newPlatform.GetComponent<MovingPlatform>();
        if (mp != null)
        {
            mp.endPoint = endPoint.position;
            mp.spawner = this;
        }

        activePlatforms.Add(newPlatform);
    }

    // Llamado por cada plataforma al llegar al destino
    public void PlatformReachedEnd(GameObject platform)
    {
        activePlatforms.Remove(platform);
        Destroy(platform);
    }
}
