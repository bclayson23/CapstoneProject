using UnityEngine;
using UnityEngine.UI;

public class CameraMonitor : MonoBehaviour
{
    [System.Serializable]
    public struct CameraFeed
    {
        public string locationName;
        public RenderTexture renderTexture;
        public RawImage displayImage;
        public BompaAI bompa; // Reference to check if Bompa is here
    }

    public CameraFeed[] cameraFeeds;
    public GameObject monitorUI;
    public Text locationText;

    private int currentCamera = 0;
    private bool isActive = false;

    void Start()
    {
        if (monitorUI != null)
            monitorUI.SetActive(false);

        UpdateCameraDisplay();
    }

    public void ToggleMonitor(bool state)
    {
        isActive = state;

        if (monitorUI != null)
            monitorUI.SetActive(state);

        if (state)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    void Update()
    {
        if (!isActive) return;

        // Switch cameras with arrow keys or numbers
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentCamera = (currentCamera + 1) % cameraFeeds.Length;
            UpdateCameraDisplay();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentCamera = (currentCamera - 1 + cameraFeeds.Length) % cameraFeeds.Length;
            UpdateCameraDisplay();
        }

        // Check for Bompa on current camera
        CheckForBompa();
    }

    void UpdateCameraDisplay()
    {
        if (cameraFeeds.Length == 0) return;

        // Update all displays
        for (int i = 0; i < cameraFeeds.Length; i++)
        {
            if (cameraFeeds[i].displayImage != null)
            {
                cameraFeeds[i].displayImage.gameObject.SetActive(i == currentCamera);

                if (i == currentCamera && locationText != null)
                {
                    locationText.text = cameraFeeds[i].locationName;
                }
            }
        }
    }

    void CheckForBompa()
    {
        if (cameraFeeds[currentCamera].bompa != null)
        {
            // Check if Bompa is visible at this location
            // You'll need to implement this based on your Bompa waypoints
        }
    }

    public int GetCurrentCamera()
    {
        return currentCamera;
    }
}