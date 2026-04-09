using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public DoorController door;

    void OnMouseDown()
    {
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null && gm.IsPowerOut())
            return;

        door.ToggleDoor();
    }
}