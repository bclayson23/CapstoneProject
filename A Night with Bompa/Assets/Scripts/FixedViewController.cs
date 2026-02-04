using UnityEngine;

public class FixedViewController : MonoBehaviour
{
    public enum ViewPosition { Center, Left, Right }

    [Header("View Positions")]
    public Transform centerView;
    public Transform leftView;
    public Transform rightView;

    [Header("Transition Settings")]
    public float transitionSpeed = 5f;

    private ViewPosition currentView = ViewPosition.Center;
    private ViewPosition targetView = ViewPosition.Center;
    private bool isTransitioning = false;

    [Header("Mouse Look (Optional)")]
    public bool allowFreeLook = false;
    public float freeLookSensitivity = 50f;
    public float freeLookLimit = 45f;

    private float freeLookRotation = 0f;
    private bool isMouseControlEnabled = true;

    void Start()
    {
        // Start at center view
        transform.position = centerView.position;
        transform.rotation = centerView.rotation;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!isMouseControlEnabled) return;

        if (allowFreeLook)
        {
            HandleFreeLook();
        }
        else
        {
            HandleFixedViewInput();
        }

        HandleViewTransition();

        // ESC to toggle cursor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleCursorLock();
        }
    }

    void HandleFixedViewInput()
    {
        // Look left (Q key or mouse left)
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetMouseButtonDown(0))
        {
            if (currentView != ViewPosition.Left)
            {
                targetView = ViewPosition.Left;
                isTransitioning = true;
            }
        }

        // Look right (E key or mouse right)
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(1))
        {
            if (currentView != ViewPosition.Right)
            {
                targetView = ViewPosition.Right;
                isTransitioning = true;
            }
        }

        // Look center (spacebar or middle mouse)
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(2))
        {
            if (currentView != ViewPosition.Center)
            {
                targetView = ViewPosition.Center;
                isTransitioning = true;
            }
        }
    }

    void HandleFreeLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * freeLookSensitivity * Time.deltaTime;
        freeLookRotation += mouseX;
        freeLookRotation = Mathf.Clamp(freeLookRotation, -freeLookLimit, freeLookLimit);

        // Apply rotation based on current view
        Quaternion baseRotation = GetBaseRotationForView(currentView);
        transform.rotation = baseRotation * Quaternion.Euler(0f, freeLookRotation, 0f);
    }

    void HandleViewTransition()
    {
        if (!isTransitioning) return;

        Transform targetTransform = GetTransformForView(targetView);

        // Smoothly move to target position/rotation
        transform.position = Vector3.Lerp(
            transform.position,
            targetTransform.position,
            Time.deltaTime * transitionSpeed
        );

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetTransform.rotation,
            Time.deltaTime * transitionSpeed
        );

        // Check if we've reached the target
        float distance = Vector3.Distance(transform.position, targetTransform.position);
        if (distance < 0.01f)
        {
            isTransitioning = false;
            currentView = targetView;
            freeLookRotation = 0f; // Reset free look when changing views
        }
    }

    Transform GetTransformForView(ViewPosition view)
    {
        switch (view)
        {
            case ViewPosition.Left: return leftView;
            case ViewPosition.Right: return rightView;
            default: return centerView;
        }
    }

    Quaternion GetBaseRotationForView(ViewPosition view)
    {
        return GetTransformForView(view).rotation;
    }

    public void SetMouseControl(bool enabled)
    {
        isMouseControlEnabled = enabled;

        if (enabled)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void ToggleCursorLock()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
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

    // Public methods to change view (can be called from UI buttons)
    public void LookLeft()
    {
        if (currentView != ViewPosition.Left)
        {
            targetView = ViewPosition.Left;
            isTransitioning = true;
        }
    }

    public void LookRight()
    {
        if (currentView != ViewPosition.Right)
        {
            targetView = ViewPosition.Right;
            isTransitioning = true;
        }
    }

    public void LookCenter()
    {
        if (currentView != ViewPosition.Center)
        {
            targetView = ViewPosition.Center;
            isTransitioning = true;
        }
    }

    public ViewPosition GetCurrentView()
    {
        return currentView;
    }
}