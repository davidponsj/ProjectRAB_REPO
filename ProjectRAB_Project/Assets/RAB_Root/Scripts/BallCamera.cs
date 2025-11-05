using Unity.Cinemachine;
using UnityEngine;

public class BallCamera : MonoBehaviour
{
    public Transform ballHolder;
    public CinemachineCamera vCam;

    private void Update()
    {
        Transform activeBall = GetActiveBall();

        if (activeBall != null && vCam.Follow != activeBall)
        {
            vCam.Follow = activeBall;
            vCam.LookAt = activeBall;
        }
    }

    private Transform GetActiveBall()
    {
        foreach (Transform ball in ballHolder)
        {
            if (ball.gameObject.activeSelf)
                return ball;
        }
        return null;
    }
}
