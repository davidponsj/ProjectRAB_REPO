using UnityEngine;

/// <summary>
/// FireSkinAttacher:
/// - Instancia un prefab de fuego como hijo del objeto objetivo (por ejemplo el "slot" de la calavera).
/// - Si el objetivo tiene MeshFilter/MeshRenderer asigna shape.mesh = mesh (estático).
/// - Si el objetivo tiene SkinnedMeshRenderer hace bake cada frame para que las partículas emitan desde la malla deformada.
/// Uso: arrastra este componente al GameObject "skull slot" o llámalo desde el código cuando cambies skin.
/// </summary>
public class FireSkinAttacher : MonoBehaviour
{
    [Tooltip("Prefab (Assets) que contiene el ParticleSystem configurado visualmente.")]
    public GameObject firePrefab;

    [Tooltip("Offset local para colocar el efecto con respecto al transform padre.")]
    public Vector3 localPositionOffset = Vector3.zero;
    public Vector3 localEulerOffset = Vector3.zero;
    public Vector3 localScale = default(Vector3);

    GameObject instance;
    ParticleSystem ps;
    Mesh bakedMesh;
    SkinnedMeshRenderer bakedFromSmr;

    void Reset()
    {
        localScale = Vector3.one;
    }

    /// <summary>
    /// Instancia o reusa el prefab y lo configura para emitir desde meshTarget.
    /// Llama a este método cuando asignes una nueva mesh/skin.
    /// </summary>
    /// <param name="meshTarget">La mesh asset para MeshFilter (puede ser null si vas a usar SkinnedMeshRenderer)</param>
    /// <param name="skinnedRenderer">SkinnedMeshRenderer si la skin es deformable (puede ser null)</param>
    public void Attach(Mesh meshTarget, SkinnedMeshRenderer skinnedRenderer)
    {
        if (firePrefab == null)
        {
            Debug.LogError("[FireSkinAttacher] firePrefab NO asignado.");
            return;
        }

        // destruir instancia vieja si existe
        if (instance != null)
        {
            Destroy(instance);
            instance = null;
            ps = null;
        }

        // instanciar prefab como hijo (local)
        instance = Instantiate(firePrefab, transform);
        instance.name = "AutoFireInstance";
        instance.transform.localPosition = localPositionOffset;
        instance.transform.localEulerAngles = localEulerOffset;
        instance.transform.localScale = localScale == Vector3.zero ? Vector3.one : localScale;

        // buscar ParticleSystem dentro
        ps = instance.GetComponentInChildren<ParticleSystem>();
        if (ps == null)
        {
            Debug.LogError("[FireSkinAttacher] No hay ParticleSystem dentro del prefab instanciado.");
            return;
        }

        // asegurar parametros básicos
        var main = ps.main;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;
        main.playOnAwake = true;

        // Si tenemos SkinnedMeshRenderer -> necesitamos bake continuo
        if (skinnedRenderer != null)
        {
            // crear mesh para bakeo
            bakedMesh = new Mesh();
            bakedFromSmr = skinnedRenderer;

            // añadir componente auxiliar para hacer bake en LateUpdate
            var continuous = instance.GetComponent<ContinuousSkinnedEmitter>();
            if (continuous == null) continuous = instance.AddComponent<ContinuousSkinnedEmitter>();
            continuous.Init(ps, skinnedRenderer);

            Debug.Log("[FireSkinAttacher] Attach hecho usando SkinnedMeshRenderer (bake continuo activado).");
            return;
        }

        // Si tenemos meshTarget (mesh estática) -> asignarla al Shape del ParticleSystem
        if (meshTarget != null)
        {
            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Mesh;
            shape.mesh = meshTarget;
            // opcional: emitir inmediatamente
            ps.Clear();
            ps.Play();
            Debug.Log("[FireSkinAttacher] Attach hecho: asignada mesh static al Shape del ParticleSystem.");
            return;
        }

        // Si no hay mesh ni skinned -> solo instanciamos y dejamos el PS con su shape original
        Debug.LogWarning("[FireSkinAttacher] Attach hecho pero no se proporcionó mesh ni SkinnedMeshRenderer. El PS usa su configuración por defecto.");
    }

    /// <summary>
    /// Llama para quitar el efecto.
    /// </summary>
    public void Detach()
    {
        if (instance != null)
        {
            Destroy(instance);
            instance = null;
            ps = null;
            if (bakedMesh != null) { Destroy(bakedMesh); bakedMesh = null; }
        }
    }

    // Clase auxiliar incluida abajo en el mismo archivo:
    // hace BakeMesh cada LateUpdate y asigna la malla baked al shape del PS.
    [RequireComponent(typeof(ParticleSystem))]
    public class ContinuousSkinnedEmitter : MonoBehaviour
    {
        ParticleSystem targetPS;
        SkinnedMeshRenderer smr;
        Mesh baked;

        public void Init(ParticleSystem ps, SkinnedMeshRenderer targetSmr)
        {
            targetPS = ps;
            smr = targetSmr;
            if (baked == null) baked = new Mesh();
            // asegurar Simulation Space Local
            var main = targetPS.main;
            main.simulationSpace = ParticleSystemSimulationSpace.Local;
            targetPS.Play();
        }

        void LateUpdate()
        {
            if (smr == null || targetPS == null) return;
            smr.BakeMesh(baked);
            var shape = targetPS.shape;
            shape.shapeType = ParticleSystemShapeType.Mesh;
            shape.mesh = baked;
        }

        void OnDestroy()
        {
            if (baked != null) Destroy(baked);
        }
    }
}