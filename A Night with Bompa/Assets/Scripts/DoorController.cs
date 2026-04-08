using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator doorAnimator;
    public Light doorLight;
    public bool isClosed = false;
    public bool lightOn = false;

    public void ToggleDoor()
    {
        if (!enabled) return;

        isClosed = !isClosed;
        doorAnimator.SetBool("IsClosed", isClosed);
    }

    public void ToggleLight(bool state)
    {
        if (!enabled) return;

        if (doorLight != null)
            doorLight.enabled = state;

        lightOn = state;
    }
}