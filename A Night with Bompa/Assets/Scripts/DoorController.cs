using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator doorAnimator;
    public Light doorLight;
    public bool isClosed = false;
    public bool lightOn = false;

    [Header("Audio")]
    public AudioSource doorAudioSource;

    public AudioClip openSound;
    public AudioClip closeSound;

    void Start()
    {
        if (doorAnimator == null)
            doorAnimator = GetComponent<Animator>();
    }

    public void ToggleDoor()
    {
        if (!enabled) return;

        if (IsAnimationPlaying()) return;

        isClosed = !isClosed;
        doorAnimator.SetBool("IsClosed", isClosed);

        if (doorAudioSource != null)
        {
            if (isClosed && closeSound != null)
            {
                doorAudioSource.PlayOneShot(closeSound);
            }
            else if (!isClosed && openSound != null)
            {
                doorAudioSource.PlayOneShot(openSound);
            }
        }
    }

    bool IsAnimationPlaying()
    {
        AnimatorStateInfo state = doorAnimator.GetCurrentAnimatorStateInfo(0);

        return state.normalizedTime < 1f;
    }

    public void ToggleLight(bool state)
    {
        if (!enabled) return;

        if (doorLight != null)
            doorLight.enabled = state;

        lightOn = state;
    }
}