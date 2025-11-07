using UnityEngine;

[DisallowMultipleComponent]
public class MeshFireAuto : MonoBehaviour
{
    [Tooltip("Lista de parejas Mesh -> Prefab de FX (arrastra aquí las meshes y los prefabs de fuego).")]
    public MeshToPrefab[] meshMappings;

    [Tooltip("Si true se buscará SkinnedMeshRenderer también (para skins animadas).")]
    public bool supportSkinned = false;

    // Nombre del hijo que creamos para el efecto
    const string instanceName = "Auto_FireFX";

    Mesh lastMesh = null;
    GameObject currentInstance = null;

    void Start()
    {
        // Inicial: forzamos comprobación en Start (si ya hay una mesh aplicada)
        CheckAndAttach();
    }

    void Update()
    {
        // Detecta cambios en la mesh (si tu sistema cambia la mesh en runtime)
        CheckAndAttach();
    }

    void CheckAndAttach()
    {
        Mesh activeMesh = GetCurrentMesh();
        if (activeMesh == lastMesh) return; // nada cambió

        lastMesh = activeMesh;
        // Si había instancia previa, la eliminamos
        if (currentInstance != null)
        {
            Destroy(currentInstance);
            currentInstance = null;
        }

        if (activeMesh == null) return;

        // buscar mapping
        GameObject prefab = FindPrefabForMesh(activeMesh);
        if (prefab == null)
        {
            // no hay efecto para esta mesh
            return;
        }

        // instantiate as child so it follows the player across scenes if the player persists
        currentInstance = Instantiate(prefab, transform);
        currentInstance.name = instanceName;
        currentInstance.transform.localPosition = Vector3.zero;
        currentInstance.transform.localRotation = Quaternion.identity;
        currentInstance.transform.localScale = Vector3.one;

        // Si el PS necesita shape.mesh (para emitir desde la malla), intentamos asignarlo:
        var ps = currentInstance.GetComponentInChildren<ParticleSystem>();
        if (ps != null)
        {
            var main = ps.main; main.simulationSpace = ParticleSystemSimulationSpace.Local; main.playOnAwake = true;
            var shape = ps.shape;
            if (shape.enabled)
            {
                // intenta asignar la mesh estática; si es Skinned hay limitación (ver nota)
                shape.shapeType = ParticleSystemShapeType.Mesh;
                shape.mesh = activeMesh;
            }
            ps.Clear(); ps.Play();
        }
    }

    Mesh GetCurrentMesh()
    {
        // Primero MeshFilter (mesh estática)
        var mf = GetComponentInChildren<MeshFilter>();
        if (mf != null && mf.sharedMesh != null) return mf.sharedMesh;

        if (supportSkinned)
        {
            var smr = GetComponentInChildren<SkinnedMeshRenderer>();
            if (smr != null && smr.sharedMesh != null) return smr.sharedMesh;
        }

        return null;
    }

    GameObject FindPrefabForMesh(Mesh m)
    {
        if (meshMappings == null) return null;
        foreach (var mp in meshMappings)
        {
            if (mp == null || mp.mesh == null) continue;
            if (mp.mesh == m) return mp.prefab;
        }
        return null;
    }

    [System.Serializable]
    public class MeshToPrefab
    {
        public Mesh mesh;
        public GameObject prefab;
    }
}
