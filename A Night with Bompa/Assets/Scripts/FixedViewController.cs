using UnityEngine;

public class FixedViewController : MonoBehaviour
{
    public Transform centerView;
    public Transform leftView;
    public Transform rightView;

    public float transitionSpeed = 8f;

    private Transform targetView;

    void Start()
    {
        targetView = centerView;

        transform.position = centerView.position;
        transform.rotation = centerView.rotation;
    }

    void Update()
    {
        // INPUT
        if (Input.GetKeyDown(KeyCode.A))
            targetView = leftView;

        if (Input.GetKeyDown(KeyCode.D))
            targetView = rightView;

        if (Input.GetKeyDown(KeyCode.W))
            targetView = centerView;

        // MOVE CAMERA
        transform.position = Vector3.Lerp(transform.position, targetView.position, Time.deltaTime * transitionSpeed);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetView.rotation, Time.deltaTime * transitionSpeed);
    }
}
