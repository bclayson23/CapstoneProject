using UnityEngine;

public class LightButton : MonoBehaviour
{
    public Light controlledLight; // Assign in Inspector
    public DoorController door;
    private bool isOn = false;

    void Start()
    {
        if (controlledLight != null)
            controlledLight.enabled = false;
    }

    void OnMouseDown()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null && gm.IsPowerOut())
            return;

        isOn = !isOn;

        if (controlledLight != null)
            controlledLight.enabled = isOn;

        if (door != null)
            door.lightOn = isOn;
    }
}