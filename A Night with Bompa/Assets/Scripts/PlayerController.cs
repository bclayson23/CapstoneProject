using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public FixedViewController lookController;
    public Camera playerCamera;

    [Header("Door Controls")]
    public DoorController leftDoor;
    public DoorController rightDoor;
    //public AudioSource doorSound;

    [Header("Camera System")]
    public CameraMonitor cameraMonitor;
    //public AudioSource staticSound;

    [Header("Power System")]
    public float maxPower = 100f;
    public float powerDrainRate = 0.5f; // per second
    private float currentPower;

    [Header("Interaction")]
    public float interactionRange = 3f;
    public LayerMask interactionLayer;

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

        // Find components if not assigned
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
        CheckInteraction();
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TryInteract();
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            if (lookController.isCenterView())
            {
                Debug.Log("Opening cameras");
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
        // Drain power for closed doors
        if (leftDoor != null && leftDoor.isClosed)
            currentPower -= powerDrainRate * Time.deltaTime;

        if (rightDoor != null && rightDoor.isClosed)
            currentPower -= powerDrainRate * Time.deltaTime;

        // Drain power for using cameras
        if (isViewingCameras)
            currentPower -= powerDrainRate * 0.5f * Time.deltaTime;

        // Drain power for door lights (optional)
        if (Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.E))
            currentPower -= powerDrainRate * 0.2f * Time.deltaTime;

        currentPower = Mathf.Clamp(currentPower, 0, maxPower);

        // Game over when power runs out
        if (currentPower <= 0 && isGameActive)
        {
            PowerOutGameOver();
        }
    }

    void PowerOutGameOver()
    {
        isGameActive = false;
        Debug.Log("Power out! Game Over!");

        // Turn off all systems
        if (leftDoor != null && leftDoor.isClosed)
            leftDoor.ToggleDoor();

        if (rightDoor != null && rightDoor.isClosed)
            rightDoor.ToggleDoor();

        if (isViewingCameras)
            ToggleCameras();

        // Show game over screen
        // Implement your game over UI here
    }

    void CheckInteraction()
    {
        // Optional: Visual feedback for interactable objects
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange, interactionLayer))
        {
            // You could highlight interactable objects here
            // Debug.DrawRay(ray.origin, ray.direction * interactionRange, Color.green);
        }
    }

    void TryInteract()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange, interactionLayer))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                interactable.Interact();
            }
        }
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

        // Disable controls
        if (lookController != null)
            lookController.enabled = false;

        // Play jumpscare sequence
        Debug.Log("Bompa got you! Game Over!");

        // Here you would:
        // 1. Play jumpscare animation
        // 2. Show game over screen
        // 3. Play scream sound
    }

    void TryOpenCameras()
    {
        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, interactionRange, interactionLayer))
        {
            if (hit.collider.CompareTag("Computer"))
            {
                ToggleCameras();
            }
        }
    }
}

// Interface for interactable objects
public interface IInteractable
{
    void Interact();
}