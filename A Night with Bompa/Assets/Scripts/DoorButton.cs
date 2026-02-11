using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public DoorController door;

    void OnMouseDown()
    {
        door.ToggleDoor();
    }
}
