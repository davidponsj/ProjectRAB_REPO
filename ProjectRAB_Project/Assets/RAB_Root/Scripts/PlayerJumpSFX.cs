using UnityEngine;

public class PlayerJumpSFX : MonoBehaviour
{
    public AudioClip jumpClip;
    public float jumpCooldown = 0.12f;
    private float lastJumpTime = -10f;

    void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            TryPlayJump();
        }
    }

    void TryPlayJump()
    {
        if (Time.time - lastJumpTime < jumpCooldown) return;
        lastJumpTime = Time.time;

        if (jumpClip == null) return;

        if (PersistentAudioManager.Instance != null)
            PersistentAudioManager.Instance.PlaySFX(jumpClip);
        else
        {
            AudioSource local = GetComponent<AudioSource>();
            if (local == null) local = gameObject.AddComponent<AudioSource>();
            local.PlayOneShot(jumpClip);
        }
    }
}
