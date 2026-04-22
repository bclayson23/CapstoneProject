using UnityEngine;
using TMPro;
using System.Collections;

public class CameraMonitor : MonoBehaviour
{
    public Camera playerCamera;
    public Camera[] cameras;
    public TextMeshProUGUI roomText;
    public GameObject monitorUI;

    private int currentCamera = 0;
    private bool isViewingCameras = false;

    public Camera cam00;
    public Camera cam01;

    public bool bompaEscaped = false;

    public TextMeshProUGUI cameraNameText;

    public GameObject blackoutScreen;

    public void SetBompaEscaped(bool state)
    {
        bompaEscaped = state;

        // If currently viewing camera 1, refresh it immediately
        if (currentCamera == 0 && isViewingCameras)
        {
            ActivateCamera(0);
        }
    }

    public void ResetMonitor()
    {
        currentCamera = 0;
        bompaEscaped = false;
        isViewingCameras = false;

        DisableAllCameras();

        if (playerCamera != null)
            playerCamera.enabled = true;

        if (monitorUI != null)
            monitorUI.SetActive(false);

        if (cameraNameText != null)
        {
            cameraNameText.gameObject.SetActive(false);
            cameraNameText.text = "CAM 01 - Cell";
        }

        if (blackoutScreen != null)
            blackoutScreen.SetActive(false);
    }

    void Start()
    {
        DisableAllCameras();

        if (monitorUI != null)
            monitorUI.SetActive(false);

        if (cameraNameText != null)
            cameraNameText.gameObject.SetActive(false);

        if (playerCamera != null)
            playerCamera.enabled = true; // make sure office camera is active
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

        cameraNameText.gameObject.SetActive(state);

        if (state)
            ActivateCamera(currentCamera);
        else
            DisableAllCameras();
    }

    void ActivateCamera(int index)
    {
        currentCamera = index;

        // Turn off all cameras
        foreach (Camera cam in cameras)
            cam.enabled = false;

        // Update text FIRST
        string[] cameraNames =
        {
        "CAM 01 - Cell",
        "CAM 02 - Cell Hall",
        "CAM 03 - Kitchen",
        "CAM 04 - Dining",
        "CAM 05 - Bathroom",
        "CAM 06 - Printer",
        "CAM 07 - Breakroom",
        "CAM 08 - Left Hall",
        "CAM 09 - Right Hall"
    };

        cameraNameText.text = cameraNames[index];

        // Special case for Cam 1
        if (index == 0)
        {
            if (!bompaEscaped)
                cam00.enabled = true;
            else
                cam01.enabled = true;

            return;
        }

        cameras[index].enabled = true;
    }

    void DisableAllCameras()
    {
        foreach (Camera cam in cameras)
            cam.enabled = false;
    }

    public IEnumerator CameraBlackout()
    {
        blackoutScreen.SetActive(true);

        yield return new WaitForSeconds(0.15f);

        blackoutScreen.SetActive(false);
    }

    public bool IsViewingCameras()
    {
        return isViewingCameras;
    }

    public void ForceClearBlackout()
    {
        StopAllCoroutines();
        blackoutScreen.SetActive(false);
    }
}