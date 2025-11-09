using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PickUpItem : MonoBehaviour
{
    public string uniqueID; // ID único de la moneda

    private void Awake()
    {
        // Genera un ID único basado en la posición de la moneda
        uniqueID = $"Coin_{transform.position.x}_{transform.position.y}_{transform.position.z}";

        // Si esta moneda ya fue recogida (PlayerPrefs = 1), no se muestra
        if (PlayerPrefs.GetInt(uniqueID, 0) == 1)
        {
            gameObject.SetActive(false);
        }
    }
}
