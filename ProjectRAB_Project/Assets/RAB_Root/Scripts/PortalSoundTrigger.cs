using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PortalSoundTriggerRobust : MonoBehaviour
{
    public AudioSource portalAudio;      // si no se asigna, se buscará en el mismo GO
    public string playerTag = "Player";
    public float maxHearDistance = 12f;  // distancia máxima a la que se seguirá oyendo por seguridad
    public bool useDistanceFallback = true; // si true, si OnTriggerExit no se recibe, se para por distancia

    Transform playerTransform;
    bool playerInsideTrigger = false;

    void Reset()
    {
        if (portalAudio == null) portalAudio = GetComponent<AudioSource>();
    }

    void Awake()
    {
        if (portalAudio == null) portalAudio = GetComponent<AudioSource>();
        portalAudio.loop = true;
        portalAudio.playOnAwake = false;

        GameObject p = GameObject.FindGameObjectWithTag(playerTag);
        if (p != null) playerTransform = p.transform;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInsideTrigger = true;
        if (!portalAudio.isPlaying) portalAudio.Play();
        Debug.Log("Portal: Player entered trigger -> audio play");
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag(playerTag)) return;
        playerInsideTrigger = false;
        if (portalAudio.isPlaying) portalAudio.Stop();
        Debug.Log("Portal: Player exited trigger -> audio stop");
    }

    void Update()
    {
        // fallback por distancia: si el trigger no funciona por algún motivo, paramos el audio si el player está lejos
        if (useDistanceFallback && playerTransform != null && portalAudio != null && portalAudio.isPlaying)
        {
            float dist = Vector3.Distance(playerTransform.position, transform.position);
            if (dist > maxHearDistance && !playerInsideTrigger)
            {
                portalAudio.Stop();
                Debug.Log("Portal: Fallback distance stop (player a " + dist + "m)");
            }
        }
    }
}
