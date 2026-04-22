using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public FixedViewController lookController;
    public Camera playerCamera;

    public DoorController leftDoor;
    public DoorController rightDoor;

    public CameraMonitor cameraMonitor;

    public float maxPower = 100f;
    public float powerDrainRate = 0.5f;
    private float currentPower;

    private bool isViewingCameras = false;
    private bool isGameActive = true;

    public void DisablePlayer()
    {
        enabled = false;

        if (lookController != null)
            lookController.enabled = false;
    }

    void Start()
    {
        currentPower = maxPower;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (lookController == null)
            lookController = GetComponent<FixedViewController>();

        if (playerCamera == null)
            playerCamera = Camera.main;

        if (Time.timeScale == 0f)
        {
            enabled = false;
        }
    }

    void Update()
    {
        if (!isGameActive) return;

        HandleInput();
        DrainPower();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (lookController.isCenterView())
            {
                ToggleCameras();
            }
        }
    }

    void ToggleCameras()
    {
        isViewingCameras = !isViewingCameras;

        if (cameraMonitor != null)
        {
            cameraMonitor.ToggleMonitor(isViewingCameras);

            BompaManager bompa = FindObjectOfType<BompaManager>();

            if (bompa != null)
            {
                if (!isViewingCameras)
                    bompa.OnCameraDown();
                else
                    bompa.OnCameraUp();
            }
        }

        if (lookController != null)
            lookController.enabled = !isViewingCameras;
    }

    void DrainPower()
    {
        if (leftDoor != null && leftDoor.isClosed)
            currentPower -= powerDrainRate * Time.deltaTime;

        if (rightDoor != null && rightDoor.isClosed)
            currentPower -= powerDrainRate * Time.deltaTime;

        if (isViewingCameras)
            currentPower -= powerDrainRate * 0.5f * Time.deltaTime;

        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
            currentPower -= powerDrainRate * 0.2f * Time.deltaTime;

        currentPower = Mathf.Clamp(currentPower, 0, maxPower);

        if (currentPower <= 0 && isGameActive)
        {
            PowerOutGameOver();
        }
    }

    void PowerOutGameOver()
    {
        isGameActive = false;

        if (leftDoor != null && leftDoor.isClosed)
            leftDoor.ToggleDoor();

        if (rightDoor != null && rightDoor.isClosed)
            rightDoor.ToggleDoor();

        if (isViewingCameras)
            ToggleCameras();
    }

    public float GetPowerPercentage()
    {
        return currentPower / maxPower;
    }

    public bool IsViewingCameras()
    {
        return isViewingCameras;
    }

    public void TriggerJumpscare()
    {
        isGameActive = false;

        if (lookController != null)
            lookController.enabled = false;
    }
}