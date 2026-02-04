using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator doorAnimator;
    public Light doorLight;
    public bool isClosed = false;

    public void ToggleDoor()
    {
        isClosed = !isClosed;
        doorAnimator.SetBool("IsClosed", isClosed);
    }

    public void ToggleLight(bool state)
    {
        if (doorLight != null)
            doorLight.enabled = state;
    }
}