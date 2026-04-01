using System.Collections;
using UnityEngine;

public class BompaManager : MonoBehaviour
{
    [Header("All Bompa Positions")]
    public GameObject bompaInCell;
    public GameObject hallBompa;

    [Header("Door References")]
    public DoorController leftDoorScript;
    public DoorController rightDoorScript;

    [Header("Attack")]
    public GameObject officeBompa;

    public GameObject[] roamingBompas; // kitchen, dining, etc

    public GameObject currentBompa;

    private bool hasEscaped = false;

    public float moveInterval = 10f;
    public float moveChance = 0.5f;

    public GameObject leftHall;
    public GameObject rightHall;
    public GameObject leftDoor;
    public GameObject rightDoor;
    public GameObject cellHall;

    private bool isAtDoor = false;
    private bool readyToAttack = false;
    private bool isLeftDoor = false;

    public float minDoorWait = 10f;
    public float maxDoorWait = 40f;

    public enum BompaState
    {
        Cell,
        CellHall,
        Roaming,
        LeftHall,
        RightHall,
        LeftDoor,
        RightDoor
    }

    private BompaState currentState;

    public void OnCameraUp()
    {
        // Player raised camera
        if (isAtDoor && !readyToAttack)
        {
            readyToAttack = true;
            Debug.Log("Bompa is ready to attack next time...");
        }
    }

    public void OnCameraDown()
    {
        // Player lowered camera
        if (isAtDoor && readyToAttack)
        {
            DoorController door = isLeftDoor ? leftDoorScript : rightDoorScript;

            if (!door.isClosed)
            {
                TriggerAttack();
            }
            else
            {
                // Player successfully blocked
                Debug.Log("Player blocked Bompa!");
            }
        }
    }

    void Start()
    {
        SetActiveBompa(bompaInCell);
        InvokeRepeating(nameof(TryMove), moveInterval, moveInterval);

        InvokeRepeating(nameof(TryMove), moveInterval, moveInterval);
    }

    void Update()
    {
        // TEMP TEST: press M to simulate midnight
        if (Input.GetKeyDown(KeyCode.M) && !hasEscaped)
        {
            EscapeCell();
        }
    }

    void EscapeCell()
    {
        hasEscaped = true;
        currentState = BompaState.CellHall;

        // Tell camera system Bompa escaped
        CameraMonitor camMonitor = FindObjectOfType<CameraMonitor>();
        if (camMonitor != null)
            camMonitor.SetBompaEscaped(true);

        SetActiveBompa(hallBompa);
    }

    void SetActiveBompa(GameObject newBompa)
    {
        if (currentBompa != null)
            currentBompa.SetActive(false);

        currentBompa = newBompa;
        currentBompa.SetActive(true);

        // Trigger blackout
        CameraMonitor camMonitor = FindObjectOfType<CameraMonitor>();
        if (camMonitor != null)
            StartCoroutine(camMonitor.CameraBlackout());
    }

    void TryMove()
    {
        if (!hasEscaped) return;

        float roll = Random.value;
        if (roll > moveChance) return;

        DecideNextMove();
    }

    void DecideNextMove()
    {
        switch (currentState)
        {
            case BompaState.CellHall:
                MoveToRoaming();
                return;

            case BompaState.Roaming:
                MoveFromRoaming();
                break;

            case BompaState.LeftHall:
                SetActiveBompa(leftDoor.gameObject);
                currentState = BompaState.LeftDoor;
                EnterDoorState(true);
                break;

            case BompaState.RightHall:
                SetActiveBompa(rightDoor.gameObject);
                currentState = BompaState.RightDoor;
                EnterDoorState(false);
                break;

            case BompaState.LeftDoor:
            case BompaState.RightDoor:
                // stays here until attack logic
                break;
        }
    }

    void MoveFromRoaming()
    {
        float decision = Random.value;

        // 50% stay roaming
        if (decision < 0.6f)
        {
            int index = Random.Range(0, roamingBompas.Length);
            SetActiveBompa(roamingBompas[index]);
            currentState = BompaState.Roaming;
        }
        // 20% go left hall
        else if (decision < 0.80f)
        {
            SetActiveBompa(leftHall);
            currentState = BompaState.LeftHall;
        }
        // 20% go right hall
        else
        {
            SetActiveBompa(rightHall);
            currentState = BompaState.RightHall;
        }
    }

    void MoveToRoaming()
    {
        int index = Random.Range(0, roamingBompas.Length);
        SetActiveBompa(roamingBompas[index]);
        currentState = BompaState.Roaming;
    }

    void ReturnToHall()
    {
        isAtDoor = false;
        readyToAttack = false;

        SetActiveBompa(cellHall);
        currentState = BompaState.CellHall;

        Debug.Log("Bompa returned to Cell Hall");
    }

    void TriggerAttack()
    {
        Debug.Log("Bompa attack!");

        if (currentBompa != null)
            currentBompa.SetActive(false);

        if (officeBompa != null)
            officeBompa.SetActive(true);

        // Stop movement
        CancelInvoke(nameof(TryMove));
    }

    IEnumerator DoorWaitTimer()
    {
        float waitTime = Random.Range(minDoorWait, maxDoorWait);

        Debug.Log("Bompa will wait: " + waitTime + " seconds");

        yield return new WaitForSeconds(waitTime);

        if (isAtDoor)
        {
            Debug.Log("Bompa leaves after waiting");

            ReturnToHall();
        }
    }

    void EnterDoorState(bool left)
    {
        isAtDoor = true;
        readyToAttack = false;
        isLeftDoor = left;

        Debug.Log("Bompa is at the door...");

        StartCoroutine(DoorWaitTimer());
    }
}