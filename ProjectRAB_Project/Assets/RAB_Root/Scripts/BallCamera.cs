using Unity.Cinemachine;
using UnityEngine;

public class BallCamera : MonoBehaviour
{
    [Header("Referencias")]
    public Transform ballHolder;      // Contenedor de las bolas
    public CinemachineCamera vCam;    // Nueva cámara de Cinemachine (Unity 6+)

    private void Update()
    {
        // Evita errores por referencias nulas
        if (vCam == null || ballHolder == null)
            return;

        Transform activeBall = GetActiveBall();

        if (activeBall == null)
            return;

        // Protección extra: evita error si la cámara o el target se destruyen
        if (vCam != null && activeBall != null && vCam.Follow != activeBall)
        {
            vCam.Follow = activeBall;
            vCam.LookAt = activeBall;
        }
    }

    private Transform GetActiveBall()
    {
        foreach (Transform ball in ballHolder)
        {
            if (ball == null)
                continue;

            if (ball.gameObject.activeSelf)
                return ball;
        }

        return null;
    }
}
