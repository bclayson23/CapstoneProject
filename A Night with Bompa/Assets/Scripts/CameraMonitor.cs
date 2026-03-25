using UnityEngine;
using TMPro;

public class CameraMonitor : MonoBehaviour
{
    public Camera playerCamera;
    public Camera[] cameras;
    public TextMeshProUGUI roomText;
    public GameObject monitorUI;

    private int currentCamera = 0;
    private bool isViewingCameras = false;

    void Start()
    {
        ActivateCamera(0);
        monitorUI.SetActive(false);
    }

    void Update()
    {
        if (!isViewingCameras) return;

        // Keys 1–9 to switch cameras
        for (int i = 0; i < cameras.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                ActivateCamera(i);
            }
        }
    }

    public void ToggleMonitor(bool state)
    {
        isViewingCameras = state;

        // Turn the player camera OFF when cameras are open
        if (playerCamera != null)
            playerCamera.enabled = !state;

        // Turn monitor UI on/off
        monitorUI.SetActive(state);

        if (state)
            ActivateCamera(currentCamera);
        else
            DisableAllCameras();
    }

    void ActivateCamera(int index)
    {
        currentCamera = index;

        for (int i = 0; i < cameras.Length; i++)
            cameras[i].enabled = (i == index);
    }

    void DisableAllCameras()
    {
        foreach (Camera cam in cameras)
            cam.enabled = false;
    }
}