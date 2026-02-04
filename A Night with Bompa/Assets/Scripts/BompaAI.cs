using UnityEngine;
using System.Collections.Generic;

public class BompaAI : MonoBehaviour
{
    public enum BompaState { Idle, Moving, AtDoor, Jumpscare }

    [Header("Waypoints")]
    public List<Transform> waypoints = new List<Transform>();
    public float moveSpeed = 3f;
    public float waitTime = 2f;

    [Header("Attack Settings")]
    public float doorCheckInterval = 1f;
    public float jumpscareDistance = 2f;
    public AudioClip movementSound;
    public AudioClip doorSound;
    public AudioClip jumpscareSound;

    private BompaState currentState;
    private int currentWaypoint = 0;
    private float waitTimer = 0f;
    private float doorCheckTimer = 0f;
    private AudioSource audioSource;
    private Transform player;

    [Header("Difficulty")]
    public float aggressionLevel = 1f; // Increases over time
    public float aggressionIncreaseRate = 0.1f; // Per hour

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentState = BompaState.Idle;
    }

    void Update()
    {
        aggressionLevel += aggressionIncreaseRate * Time.deltaTime / 3600f; // Increase per real hour

        switch (currentState)
        {
            case BompaState.Idle:
                HandleIdleState();
                break;

            case BompaState.Moving:
                HandleMovingState();
                break;

            case BompaState.AtDoor:
                HandleAtDoorState();
                break;

            case BompaState.Jumpscare:
                // Jumpscare animation/effect
                break;
        }

        CheckForJumpscare();
    }

    void HandleIdleState()
    {
        waitTimer += Time.deltaTime;

        if (waitTimer >= waitTime / aggressionLevel)
        {
            waitTimer = 0f;
            currentState = BompaState.Moving;

            // Move to next waypoint
            currentWaypoint++;
            if (currentWaypoint >= waypoints.Count)
                currentWaypoint = waypoints.Count - 1; // Stay at last waypoint (near office)
        }
    }

    void HandleMovingState()
    {
        if (currentWaypoint < waypoints.Count)
        {
            Vector3 targetPos = waypoints[currentWaypoint].position;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime * aggressionLevel);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                currentState = BompaState.Idle;

                // Play arrival sound if at door
                if (currentWaypoint >= waypoints.Count - 2) // Last two waypoints are near doors
                {
                    PlayDoorSound();
                    currentState = BompaState.AtDoor;
                }
            }
        }
    }

    void HandleAtDoorState()
    {
        doorCheckTimer += Time.deltaTime;

        if (doorCheckTimer >= doorCheckInterval)
        {
            doorCheckTimer = 0f;

            // Check if door is open (simplified)
            // You'll need to reference your door controllers here
            bool isDoorClosed = CheckIfDoorClosed();

            if (!isDoorClosed)
            {
                // Enter office
                currentState = BompaState.Jumpscare;
                TriggerJumpscare();
            }
        }
    }

    bool CheckIfDoorClosed()
    {
        // Implement based on your door setup
        // This should check if the corresponding door (left/right) is closed
        return Random.value > 0.5f; // Placeholder
    }

    void CheckForJumpscare()
    {
        if (Vector3.Distance(transform.position, player.position) < jumpscareDistance)
        {
            TriggerJumpscare();
        }
    }

    void TriggerJumpscare()
    {
        currentState = BompaState.Jumpscare;

        // Play jumpscare sound
        if (audioSource != null && jumpscareSound != null)
        {
            audioSource.PlayOneShot(jumpscareSound);
        }

        // Trigger jumpscare animation/UI
        Debug.Log("JUMPSCARE! Game Over!");

        // Here you would trigger game over screen
    }

    void PlayDoorSound()
    {
        if (audioSource != null && doorSound != null)
        {
            audioSource.PlayOneShot(doorSound);
        }
    }

    // Call this from camera system when Bompa is visible
    public Vector3 GetCurrentLocation()
    {
        return transform.position;
    }

    public bool IsVisibleOnCamera(int cameraIndex)
    {
        // Check if Bompa is at a location visible from specific camera
        // This depends on your waypoint setup
        return currentWaypoint == cameraIndex;
    }
}