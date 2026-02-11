using UnityEngine;

public class LightButton : MonoBehaviour
{
    public Light controlledLight; // Assign in Inspector
    private bool isOn = false;

    void Start()
    {
        if (controlledLight != null)
            controlledLight.enabled = false; // Force off at start
    }

    void OnMouseDown()
    {
        isOn = !isOn;
        if (controlledLight != null)
            controlledLight.enabled = isOn;
    }
}
