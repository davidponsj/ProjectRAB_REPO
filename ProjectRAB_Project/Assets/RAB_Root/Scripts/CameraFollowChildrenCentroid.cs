using UnityEngine;

public class CameraFollowChildrenCentroid : MonoBehaviour
{
    public Transform[] children; // arrastra manualmente o usa código para llenar
    public float smooth = 10f;
    public Vector3 offset = Vector3.zero;

    void LateUpdate()
    {
        if (children == null || children.Length == 0) return;
        Vector3 sum = Vector3.zero;
        int valid = 0;
        foreach (var c in children)
        {
            if (c == null) continue;
            sum += c.position;
            valid++;
        }
        if (valid == 0) return;
        Vector3 centroid = sum / valid + offset;
        transform.position = Vector3.Lerp(transform.position, centroid, Mathf.Clamp01(smooth * Time.deltaTime));
    }
}