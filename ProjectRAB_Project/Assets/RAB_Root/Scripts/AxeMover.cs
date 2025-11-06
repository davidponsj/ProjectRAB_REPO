using UnityEngine;

public class AxeMover : MonoBehaviour
{
    public Vector3 moveAxis = Vector3.right;
    public float amplitude = 2f;
    public float speed = 2f;
    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
        if (moveAxis == Vector3.zero) moveAxis = Vector3.right;
        moveAxis = moveAxis.normalized;
    }

    void Update()
    {
        float t = Mathf.Sin(Time.time * speed); // -1..1
        transform.position = startPos + moveAxis * (t * amplitude);
    }
}